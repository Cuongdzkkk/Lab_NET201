
// LIQUID LUXURY - THREE.JS BACKGROUND
// "The Aurora of Silicon Valley"

const canvas = document.createElement('canvas');
canvas.id = 'liquid-canvas';
canvas.style.position = 'fixed';
canvas.style.top = '0';
canvas.style.left = '0';
canvas.style.width = '100vw';
canvas.style.height = '100vh';
canvas.style.zIndex = '-1';
canvas.style.pointerEvents = 'none'; // Let clicks pass through
document.body.prepend(canvas);

// --- CONFIGURATION ---
const config = {
    color1: 0x1a0b2e, // Deep Velvet Violet
    color2: 0x0f1c3a, // Midnight Blue
    color3: 0xffd700, // Faint Gold (Accent)
    speed: 0.0005,
    amplitude: 0.3
};

// --- SCENE SETUP ---
const scene = new THREE.Scene();
scene.background = new THREE.Color(0x050508); // Very dark base

const camera = new THREE.PerspectiveCamera(75, window.innerWidth / window.innerHeight, 0.1, 100);
camera.position.z = 5;

const renderer = new THREE.WebGLRenderer({ canvas: canvas, antialias: true, alpha: true });
renderer.setSize(window.innerWidth, window.innerHeight);
renderer.setPixelRatio(Math.min(window.devicePixelRatio, 2));

// --- MESH: THE SILK ---
// High segment check for smoothness
const geometry = new THREE.PlaneGeometry(20, 20, 128, 128);

// Custom Shader for that "Silky" feel
const material = new THREE.ShaderMaterial({
    uniforms: {
        uTime: { value: 0 },
        uMouse: { value: new THREE.Vector2(0, 0) },
        uResolution: { value: new THREE.Vector2(window.innerWidth, window.innerHeight) },
        uColor1: { value: new THREE.Color(config.color1) },
        uColor2: { value: new THREE.Color(config.color2) },
        uAccent: { value: new THREE.Color(config.color3) }
    },
    vertexShader: `
        uniform float uTime;
        uniform vec2 uMouse;
        varying vec2 vUv;
        varying float vElevation;

        void main() {
            vUv = uv;

            float time = uTime * 0.2;

            // Fluid Wave Calculation (Sin/Cos overlapping)
            float elevation = sin(position.x * 0.5 + time) * 0.4;
            elevation += sin(position.y * 0.5 + time * 0.5) * 0.4;
            
            // Micro-ripples (Silk texture)
            elevation += sin(position.x * 2.0 + time * 2.0) * 0.1;
            elevation += cos(position.y * 1.5 + time * 1.5) * 0.1;

            // Mouse Interaction (Gentle push)
            float dist = distance(uv, uMouse);
            float interaction = smoothstep(0.5, 0.0, dist) * 0.5;
            elevation -= interaction; // Dip down on mouse

            vElevation = elevation;

            vec3 newPos = position;
            newPos.z += elevation;

            gl_Position = projectionMatrix * modelViewMatrix * vec4(newPos, 1.0);
        }
    `,
    fragmentShader: `
        uniform vec3 uColor1;
        uniform vec3 uColor2;
        uniform vec3 uAccent;
        varying vec2 vUv;
        varying float vElevation;

        void main() {
            // Mix colors based on elevation
            float mixStrength = (vElevation + 1.0) * 0.5;
            
            vec3 color = mix(uColor1, uColor2, mixStrength);
            
            // Highlight tips with faint gold
            float highlight = smoothstep(0.8, 1.0, mixStrength);
            color = mix(color, uAccent, highlight * 0.2); // Subtle gold

            gl_FragColor = vec4(color, 1.0);
        }
    `/*,
    wireframe: false*/
});

const mesh = new THREE.Mesh(geometry, material);
mesh.rotation.x = -Math.PI / 4; // Tilt back slightly
scene.add(mesh);

// --- LIGHTING (Fake logic with shader, but lets add ambient for sanity) ---
// Not strictly needed with ShaderMaterial but good practice if we add standard meshes later.
const ambientLight = new THREE.AmbientLight(0xffffff, 0.5);
scene.add(ambientLight);

// --- MOUSE TRACKING ---
let mouse = new THREE.Vector2(0.5, 0.5);
window.addEventListener('mousemove', (e) => {
    mouse.x = e.clientX / window.innerWidth;
    mouse.y = 1.0 - (e.clientY / window.innerHeight); // Flip Y for texture UV match
});

// --- ANIMATION ---
const clock = new THREE.Clock();

function animate() {
    requestAnimationFrame(animate);

    const elapsedTime = clock.getElapsedTime();

    material.uniforms.uTime.value = elapsedTime;

    // Smooth mouse lerp
    material.uniforms.uMouse.value.lerp(mouse, 0.05);

    renderer.render(scene, camera);
}

animate();

// --- RESIZE ---
window.addEventListener('resize', () => {
    camera.aspect = window.innerWidth / window.innerHeight;
    camera.updateProjectionMatrix();
    renderer.setSize(window.innerWidth, window.innerHeight);
    material.uniforms.uResolution.value.set(window.innerWidth, window.innerHeight);
});
