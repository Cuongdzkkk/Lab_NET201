import * as THREE from 'https://unpkg.com/three@0.160.0/build/three.module.js';

export function initRainEffect() {
    const container = document.getElementById('canvas-container');
    if (!container) return;

    const scene = new THREE.Scene();
    const camera = new THREE.OrthographicCamera(-1, 1, 1, -1, 0, 1);
    const renderer = new THREE.WebGLRenderer({ alpha: true });
    renderer.setSize(window.innerWidth, window.innerHeight);
    container.appendChild(renderer.domElement);

    // Shader for Rain on Glass
    const material = new THREE.ShaderMaterial({
        uniforms: {
            u_time: { value: 0 },
            u_resolution: { value: new THREE.Vector2(window.innerWidth, window.innerHeight) },
            u_color_1: { value: new THREE.Color(0x0a192f) }, // Dark Moody Blue
            u_color_2: { value: new THREE.Color(0x112240) }  // Deep Grey/Blue
        },
        vertexShader: `
            varying vec2 vUv;
            void main() {
                vUv = uv;
                gl_Position = vec4(position, 1.0);
            }
        `,
        fragmentShader: `
            uniform float u_time;
            uniform vec2 u_resolution;
            uniform vec3 u_color_1;
            uniform vec3 u_color_2;
            varying vec2 vUv;

            // Pseudo-random function
            float hash(vec2 p) {
                return fract(sin(dot(p, vec2(12.9898, 78.233))) * 43758.5453);
            }

            // Simple noise
            float noise(vec2 p) {
                vec2 i = floor(p);
                vec2 f = fract(p);
                f = f * f * (3.0 - 2.0 * f);
                return mix(mix(hash(i + vec2(0.0, 0.0)), hash(i + vec2(1.0, 0.0)), f.x),
                           mix(hash(i + vec2(0.0, 1.0)), hash(i + vec2(1.0, 1.0)), f.x), f.y);
            }

            // Rain streaks function
            float rain(vec2 uv) {
                vec2 aspect = vec2(2.0, 1.0); 
                float t = u_time * 0.8;
                
                vec2 uv_rain = uv * vec2(20.0, 1.0); // Stretch vertically
                uv_rain.y += t * 10.0;
                
                float n = noise(uv_rain);
                // Threshold to make distinct drops/streaks
                return smoothstep(0.6, 1.0, n) * 0.5; 
            }
            
            // Droplets sliding
            float droplets(vec2 uv) {
                float t = u_time * 0.2;
                vec2 grid = vec2(10.0, 4.0);
                vec2 id = floor(uv * grid);
                vec3 rand = vec3(hash(id), hash(id + 1.0), hash(id + 2.0));
                
                float slideSpeed = rand.x * 2.0 + 0.5;
                float yOffset = fract(t * slideSpeed + rand.y);
                
                vec2 localUv = fract(uv * grid) - 0.5;
                
                // Distort position based on yOffset to look like sliding
                localUv.y += (yOffset - 0.5) * 2.0; 
                
                float d = length(localUv);
                
                // Mask drops that are "falling"
                if (d < 0.3) return 0.2; // Visual strength
                return 0.0;
            }

            void main() {
                // Background gradient with noise
                float n = noise(vUv * 3.0 + u_time * 0.05);
                vec3 bg = mix(u_color_1, u_color_2, vUv.y + n * 0.2);

                // Add rain streaks
                float rainVal = rain(vUv);
                
                // Add some "blur" / glow to rain
                vec3 rainColor = vec3(0.6, 0.7, 0.8);
                
                // Final composition
                vec3 color = bg + rainColor * rainVal * 0.1;
                
                gl_FragColor = vec4(color, 1.0);
            }
        `,
        transparent: true
    });

    const geometry = new THREE.PlaneGeometry(2, 2);
    const plane = new THREE.Mesh(geometry, material);
    scene.add(plane);

    window.addEventListener('resize', () => {
        renderer.setSize(window.innerWidth, window.innerHeight);
        material.uniforms.u_resolution.value.set(window.innerWidth, window.innerHeight);
    });

    function animate(time) {
        requestAnimationFrame(animate);
        material.uniforms.u_time.value = time * 0.001;
        renderer.render(scene, camera);
    }
    animate(0);
}
