document.addEventListener('DOMContentLoaded', () => {
    // 1. Page Transitions (System Loader)
    setupPageTransitions();

    // 2. Structured Infinite Ocean
    initInfiniteOcean();

    // 3. Identity Text Decoder
    const hackerText = document.getElementById('hacker-text');
    if (hackerText) {
        new TextScramble(hackerText, "HELLO, DOAN QUOC CUONG").start();
    }

    // 4. Dynamic Forms
    initDynamicForms();

    // 5. Focus Handling
    setupFocusHandling();
});

let isUiFocused = false;

// === PAGE TRANSITIONS ===
function setupPageTransitions() {
    const loader = document.getElementById('loading-overlay');

    setTimeout(() => {
        if (loader) {
            loader.classList.add('hidden');
            loader.classList.remove('active');
        }
    }, 500);

    document.querySelectorAll('a').forEach(link => {
        link.addEventListener('click', (e) => {
            const href = link.getAttribute('href');
            if (!href || href.startsWith('#') || link.target === '_blank') return;

            e.preventDefault();
            if (loader) {
                loader.classList.remove('hidden');
                loader.classList.add('active');
            }
            setTimeout(() => { window.location.href = href; }, 800);
        });
    });
}

function setupFocusHandling() {
    const cards = document.querySelectorAll('.bento-card, .dock-container, form, .hud-card, .tech-table');
    cards.forEach(c => {
        c.addEventListener('mouseenter', () => isUiFocused = true);
        c.addEventListener('mouseleave', () => isUiFocused = false);
    });
}

// === THREE.JS INFINITE LIVING OCEAN ===
function initInfiniteOcean() {
    const container = document.getElementById('canvas-container');
    if (!container) return;

    const scene = new THREE.Scene();

    // Camera Setup for Wide Coverage
    const camera = new THREE.PerspectiveCamera(60, window.innerWidth / window.innerHeight, 1, 2000);
    // Positioned further back and higher to view the massive grid
    camera.position.set(0, -100, 100);
    camera.lookAt(0, 0, 0);

    const renderer = new THREE.WebGLRenderer({ alpha: true, antialias: true });
    renderer.setSize(window.innerWidth, window.innerHeight);
    renderer.setPixelRatio(window.devicePixelRatio);
    container.appendChild(renderer.domElement);

    // Plane Geometry - ULTRA MASSIVE SIZE for Full Coverage
    // Size 1500 ensures it covers absolutely everything
    const size = 1500;
    const segments = 150; // Increased resolution
    const geometry = new THREE.PlaneGeometry(size, size, segments, segments);

    const posAttribute = geometry.attributes.position;
    const originalPositions = posAttribute.array.slice();

    const material = new THREE.MeshBasicMaterial({
        color: 0x00f3ff,
        wireframe: true,
        transparent: true,
        opacity: 0.15
    });

    const mesh = new THREE.Mesh(geometry, material);
    scene.add(mesh);

    // Interaction
    const raycaster = new THREE.Raycaster();
    const mouse = new THREE.Vector2(-100, -100);
    const plane = new THREE.Plane(new THREE.Vector3(0, 0, 1), 0);

    document.addEventListener('mousemove', e => {
        mouse.x = (e.clientX / window.innerWidth) * 2 - 1;
        mouse.y = -(e.clientY / window.innerHeight) * 2 + 1;
    });

    window.addEventListener('resize', () => {
        camera.aspect = window.innerWidth / window.innerHeight;
        camera.updateProjectionMatrix();
        renderer.setSize(window.innerWidth, window.innerHeight);
    });

    // SMOOTHING VARIABLES
    let currentDamp = 1.0;

    function animate() {
        requestAnimationFrame(animate);
        const time = Date.now() * 0.001;

        // LERP DAMPENING (Smooth Transition)
        // If focused, target is 0.1 (calm). If not, 1.0 (active).
        // 0.05 is the lerp speed (lower = smoother/slower)
        const targetDamp = isUiFocused ? 0.1 : 1.0;
        currentDamp += (targetDamp - currentDamp) * 0.05;

        raycaster.setFromCamera(mouse, camera);
        const intersect = new THREE.Vector3();
        raycaster.ray.intersectPlane(plane, intersect);

        const count = posAttribute.count;
        for (let i = 0; i < count; i++) {
            const ox = originalPositions[i * 3];
            const oy = originalPositions[i * 3 + 1];

            // 1. Ambient Wave (Breathing)
            // We multiply by currentDamp primarily to calm it down when reading
            let z = Math.sin(ox * 0.05 + time) * Math.cos(oy * 0.05 + time * 0.8) * 3.0;
            z += Math.sin(ox * 0.15 + time * 1.5) * 1.0;

            // 2. Mouse Magnetic
            // Only apply if looking at it (damp > 0.5) to avoid distraction
            if (intersect) {
                const dx = intersect.x - ox;
                const dy = intersect.y - oy;
                const dist = Math.sqrt(dx * dx + dy * dy);

                // Mouse effect fades out as damp decreases
                if (dist < 50) {
                    const force = (1 - dist / 50) * 20 * currentDamp;
                    z += force;
                }
            }

            // Apply dampening to the final Z height
            // We dampen the AMPLITUDE of the wave equal to focus state
            posAttribute.setZ(i, z * currentDamp);
        }

        posAttribute.needsUpdate = true;
        renderer.render(scene, camera);
    }
    animate();
}

// === UTILS ===
class TextScramble {
    constructor(el, t) { this.e = el; this.t = t; this.c = '!<>-_\\/[]{}—=+*^?#________'; this.r = 0; this.f = 0; }
    start() { this.u(); }
    u() {
        let o = '';
        for (let i = 0; i < this.t.length; i++) {
            if (i < this.r) o += this.t[i]; else o += this.c[Math.floor(Math.random() * this.c.length)];
        }
        this.e.innerText = o;
        if (this.r < this.t.length) { this.f++; if (this.f % 2 === 0) this.r++; requestAnimationFrame(() => this.u()); }
    }
}

function initDynamicForms() {
    const btn = document.getElementById('addDetailBtn');
    if (!btn) return;

    window.addItem = function () {
        const c = document.getElementById('inventory-list');
        const i = c.querySelectorAll('.inventory-row').length;
        const r = document.createElement('div');
        r.className = 'inventory-row';
        r.style.cssText = "display:grid;grid-template-columns:2fr 1fr 1fr auto;gap:1rem;margin-bottom:1rem;align-items:end;opacity:0;transform:translateY(-10px);transition:all 0.3s ease;";
        r.innerHTML = `
            <div><label class="form-label">ITEM_ID</label><input type="text" name="OrderDetails[${i}].ProductName" class="form-control" required/></div>
            <div><label class="form-label">QTY</label><input type="number" name="OrderDetails[${i}].Quantity" class="form-control" required min="1"/></div>
            <div><label class="form-label">COST</label><input type="number" name="OrderDetails[${i}].Price" class="form-control" required step="0.01"/></div>
            <button type="button" class="btn btn-pink btn-sm" onclick="this.parentElement.remove()">X</button>
        `;
        c.appendChild(r);
        setTimeout(() => { r.style.opacity = 1; r.style.transform = "translateY(0)"; }, 10);
    };
    btn.addEventListener('click', addItem);
    if (document.getElementById('inventory-list').children.length === 0) addItem();
}
