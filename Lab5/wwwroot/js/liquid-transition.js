/* ============================================
   LIQUID TRANSITION - Natural Water Fill
   Smooth 800-1000ms with natural blue gradient
   ============================================ */

document.addEventListener("DOMContentLoaded", () => {
    const overlay = document.getElementById('liquid-overlay');

    if (!overlay) return;

    // --- PAGE LOAD ANIMATION (Entrance) ---
    // Start with overlay covering screen
    overlay.style.transition = 'none';
    overlay.style.transform = 'translateY(0%)';

    // Trigger reflow
    overlay.offsetHeight;

    // Animate down to reveal content
    setTimeout(() => {
        overlay.style.transition = 'transform 1s cubic-bezier(0.4, 0.0, 0.2, 1)';
        overlay.style.transform = 'translateY(100%)';
    }, 150);

    // --- PAGE EXIT ANIMATION (Navigation) ---
    document.querySelectorAll('a').forEach(link => {
        link.addEventListener('click', (e) => {
            const href = link.getAttribute('href');

            // Skip hash links, external, blank targets, and scripts
            if (!href ||
                href.startsWith('#') ||
                link.target === '_blank' ||
                href.startsWith('javascript:') ||
                href.startsWith('http')) {
                return;
            }

            // Skip if same page
            if (href === window.location.pathname) return;

            e.preventDefault();

            // Water fill animation - Rise up
            overlay.style.transition = 'transform 0.9s cubic-bezier(0.4, 0.0, 0.2, 1)';
            overlay.style.transform = 'translateY(0%)';

            // Navigate after animation
            setTimeout(() => {
                window.location.href = href;
            }, 900);
        });
    });

    // --- FORM SUBMISSION ANIMATION ---
    document.querySelectorAll('form').forEach(form => {
        form.addEventListener('submit', (e) => {
            // Let form submit naturally, just show visual feedback
            overlay.style.transition = 'transform 0.6s cubic-bezier(0.4, 0.0, 0.2, 1)';
            overlay.style.transform = 'translateY(0%)';
        });
    });
});
