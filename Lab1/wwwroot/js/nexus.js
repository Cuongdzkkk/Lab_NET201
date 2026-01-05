/* NEXUS INTERACTION ENGINE */

document.addEventListener('DOMContentLoaded', () => {

    // 1. Spotlight Effect (Mouse Tracking Borders)
    const cards = document.querySelectorAll('.bento-card');

    document.addEventListener('mousemove', (e) => {
        cards.forEach(card => {
            const rect = card.getBoundingClientRect();
            const x = e.clientX - rect.left;
            const y = e.clientY - rect.top;

            card.style.setProperty('--mouse-x', `${x}px`);
            card.style.setProperty('--mouse-y', `${y}px`);
        });
    });

    // 2. 3D Tilt Effect
    cards.forEach(card => {
        card.addEventListener('mousemove', (e) => {
            const rect = card.getBoundingClientRect();
            const x = e.clientX - rect.left; // x position within the element.
            const y = e.clientY - rect.top;  // y position within the element.

            const centerX = rect.width / 2;
            const centerY = rect.height / 2;

            const rotateX = ((y - centerY) / centerY) * -5; // Max rotation deg
            const rotateY = ((x - centerX) / centerX) * 5;

            card.style.transform = `perspective(1000px) rotateX(${rotateX}deg) rotateY(${rotateY}deg) scale3d(1.02, 1.02, 1.02)`;
        });

        card.addEventListener('mouseleave', () => {
            card.style.transform = 'perspective(1000px) rotateX(0) rotateY(0) scale3d(1, 1, 1)';
        });
    });

    // 3. Real-time Clock
    const updateTime = () => {
        const timeElement = document.getElementById('nexus-time');
        if (timeElement) {
            const now = new Date();
            timeElement.innerText = now.toLocaleTimeString('en-US', { hour12: false });
        }
    };
    setInterval(updateTime, 1000);
    updateTime();

    // 4. Fake CPU Stats
    const updateCpu = () => {
        const cpuEl = document.getElementById('cpu-stat');
        if (cpuEl) {
            const load = Math.floor(Math.random() * 30) + 10;
            cpuEl.innerText = `${load}%`;
        }
    }
    setInterval(updateCpu, 2000);

    // 5. Page Transition Logic
    const loader = document.getElementById('nexus-loader');
    const content = document.getElementById('main-content');

    // Fade IN on load
    if (loader && content) {
        // Initial state set in HTML is opacity-0 for loader, so it's hidden by default if JS runs fast
        // But to prevent flash, we might want it visible initially via CSS, then remove.
        // For "gliding" feel between pages:

        // Browser handles restoration, we just need to ensure enter animation plays
        content.classList.remove('opacity-0');
        content.classList.add('opacity-100');
    }

    // Intercept Links for Fade OUT
    document.addEventListener('click', (e) => {
        const link = e.target.closest('a');
        if (link && link.href && !link.href.startsWith('#') && !link.target && link.hostname === window.location.hostname) {
            e.preventDefault();
            const href = link.href;

            // Show Loader
            if (loader) {
                loader.classList.remove('opacity-0');
                loader.classList.add('opacity-100');
            }
            if (content) {
                content.classList.remove('opacity-100');
                content.classList.add('opacity-0');
            }

            // Wait for animation then navigate
            setTimeout(() => {
                window.location.href = href;
            }, 600);
        }
    });

});
