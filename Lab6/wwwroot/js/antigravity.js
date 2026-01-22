/**
 * ANTIGRAVITY v2.0
 * Google Deepmind Aesthetic Clone
 * 
 * Features:
 * - Floating Geometries (Torus, Cone, Icosahedron, Pill)
 * - Google Brand Colors
 * - Bubbling "Champagne" Physics (Lift-off)
 * - Mouse Repulsion
 * - Infinite Formatting Loop
 */

const CONFIG = {
    particleCount: 60, // Total number of floating objects
    floatSpeedBase: 0.05,
    floatSpeedVar: 0.1,
    rotationSpeed: 0.005,
    repulsionRadius: 100, // Distance to react to mouse
    repulsionForce: 2.5,   // How hard they fly away
    resetHeight: 80,      // Y position to reset to bottom
    spawnY: -80,          // Start position
    colors: [
        0x4285F4, // Google Blue
        0xEA4335, // Google Red
        0xFBBC05, // Google Yellow
        0x34A853, // Google Green
        0xF1F3F4  // Google Grey (Background/Subtle)
    ]
};

class AntigravityScene {
    constructor() {
        this.container = document.getElementById('canvas-container');
        this.objects = [];
        this.mouse = new THREE.Vector2(-9999, -9999); // Off-screen initially
        this.init();
    }

    init() {
        // 1. Setup Scene
        this.scene = new THREE.Scene();
        this.scene.fog = new THREE.FogExp2(0xffffff, 0.002); // White fog

        // 2. Camera
        this.camera = new THREE.PerspectiveCamera(75, window.innerWidth / window.innerHeight, 0.1, 1000);
        this.camera.position.z = 100;

        // 3. Renderer
        this.renderer = new THREE.WebGLRenderer({ alpha: true, antialias: true });
        this.renderer.setSize(window.innerWidth, window.innerHeight);
        this.renderer.setPixelRatio(window.devicePixelRatio);
        this.container.appendChild(this.renderer.domElement);

        // 4. Lights
        const ambientLight = new THREE.AmbientLight(0xffffff, 0.7);
        this.scene.add(ambientLight);

        const dirLight = new THREE.DirectionalLight(0xffffff, 0.6);
        dirLight.position.set(20, 50, 20);
        this.scene.add(dirLight);

        // 5. Create Objects
        this.createFloatingObjects();

        // 6. Events
        window.addEventListener('resize', this.onResize.bind(this));
        document.addEventListener('mousemove', this.onMouseMove.bind(this));

        // 7. Start Loop
        this.animate();
    }

    createFloatingObjects() {
        // Geometries
        const geometries = [
            new THREE.TorusGeometry(3, 1, 16, 32),
            new THREE.ConeGeometry(3, 6, 32),
            new THREE.IcosahedronGeometry(3.5, 0),
            // Pill/Capsule approximation (Scaled Sphere)
            new THREE.SphereGeometry(3, 32, 32)
        ];

        // Material (Matte, plastic-like)
        const materialRaw = new THREE.MeshStandardMaterial({
            roughness: 0.4,
            metalness: 0.1,
            flatShading: false
        });

        for (let i = 0; i < CONFIG.particleCount; i++) {
            // Random Geometry
            const geoIndex = Math.floor(Math.random() * geometries.length);
            const geometry = geometries[geoIndex];

            // Random Color
            const color = CONFIG.colors[Math.floor(Math.random() * CONFIG.colors.length)];
            const material = materialRaw.clone();
            material.color.setHex(color);

            // Mesh
            const mesh = new THREE.Mesh(geometry, material);

            // If it's the Sphere (Index 3), stretch it to make a Pill
            if (geoIndex === 3) {
                mesh.scale.set(1, 1.8, 1);
            }

            // Scatter Setup
            this.resetParticle(mesh);
            // Randomize starting Y so they don't all start at bottom
            mesh.position.y = (Math.random() * (CONFIG.resetHeight - CONFIG.spawnY)) + CONFIG.spawnY;

            // Custom Data for Physics
            mesh.userData = {
                velocity: new THREE.Vector3(0, CONFIG.floatSpeedBase + Math.random() * CONFIG.floatSpeedVar, 0),
                rotSpeed: {
                    x: (Math.random() - 0.5) * CONFIG.rotationSpeed,
                    y: (Math.random() - 0.5) * CONFIG.rotationSpeed
                }
            };

            this.scene.add(mesh);
            this.objects.push(mesh);
        }
    }

    resetParticle(mesh) {
        // Reset to bottom, random X/Z
        mesh.position.y = CONFIG.spawnY;
        mesh.position.x = (Math.random() - 0.5) * 200; // Wide spread
        mesh.position.z = (Math.random() - 0.5) * 100; // Depth
    }

    onMouseMove(event) {
        // Normalize mouse to -1 to 1 (NDC)
        this.mouse.x = (event.clientX / window.innerWidth) * 2 - 1;
        this.mouse.y = -(event.clientY / window.innerHeight) * 2 + 1;
    }

    onResize() {
        this.camera.aspect = window.innerWidth / window.innerHeight;
        this.camera.updateProjectionMatrix();
        this.renderer.setSize(window.innerWidth, window.innerHeight);
    }

    animate() {
        requestAnimationFrame(this.animate.bind(this));

        // Get Mouse Position in World Space (Approximate at z=0 plane)
        // We project a vector from camera
        const vector = new THREE.Vector3(this.mouse.x, this.mouse.y, 0.5);
        vector.unproject(this.camera);
        const dir = vector.sub(this.camera.position).normalize();
        const distance = -this.camera.position.z / dir.z;
        const mouseWorldPos = this.camera.position.clone().add(dir.multiplyScalar(distance));

        this.objects.forEach(obj => {
            // 1. Float Up
            obj.position.add(obj.userData.velocity);

            // 2. Rotate
            obj.rotation.x += obj.userData.rotSpeed.x;
            obj.rotation.y += obj.userData.rotSpeed.y;

            // 3. Reset if out of bounds
            if (obj.position.y > CONFIG.resetHeight) {
                this.resetParticle(obj);
            }

            // 4. Mouse Repulsion
            const dist = obj.position.distanceTo(mouseWorldPos);
            if (dist < 40) { // Interaction radius
                const repulseDir = obj.position.clone().sub(mouseWorldPos).normalize();
                // Push away
                obj.position.add(repulseDir.multiplyScalar(0.5));
                // Add some spin
                obj.rotation.x += 0.1;
                obj.rotation.y += 0.1;
            }

            // Apply slight return-to-center drift for X (optional, keeps them on screen)
            // obj.position.x *= 0.999; 
        });

        this.renderer.render(this.scene, this.camera);
    }
}

// Initialize on Load
window.addEventListener('DOMContentLoaded', () => {
    new AntigravityScene();
});
