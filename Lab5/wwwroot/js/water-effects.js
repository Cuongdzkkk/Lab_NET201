/* ============================================
   WATER EFFECTS - Ripples & Interactions
   ============================================ */

document.addEventListener('DOMContentLoaded', () => {

    // --- RIPPLE EFFECT ON CLICK ---
    function createRipple(event) {
        const target = event.currentTarget;
        const ripple = document.createElement('span');
        const rect = target.getBoundingClientRect();

        const size = Math.max(rect.width, rect.height);
        const x = event.clientX - rect.left - size / 2;
        const y = event.clientY - rect.top - size / 2;

        ripple.style.width = ripple.style.height = size + 'px';
        ripple.style.left = x + 'px';
        ripple.style.top = y + 'px';
        ripple.classList.add('ripple-effect');

        target.appendChild(ripple);

        // Remove after animation
        setTimeout(() => ripple.remove(), 600);
    }

    // Add ripple to interactive elements
    const rippleElements = document.querySelectorAll('.bento-box, .btn, .btn-primary, .btn-rain, button');
    rippleElements.forEach(el => {
        // Ensure relative position for ripple
        if (getComputedStyle(el).position === 'static') {
            el.style.position = 'relative';
        }
        el.addEventListener('click', createRipple);
    });

    // --- CSS for ripple effect (inject into head) ---
    const style = document.createElement('style');
    style.textContent = `
        .ripple-effect {
            position: absolute;
            border-radius: 50%;
            background: rgba(0, 188, 212, 0.4);
            transform: scale(0);
            animation: ripple-animation 0.6s ease-out;
            pointer-events: none;
        }
        
        @keyframes ripple-animation {
            to {
                transform: scale(2);
                opacity: 0;
            }
        }
    `;
    document.head.appendChild(style);


    // --- WATER DROPLET TRAILS (Decorative) ---
    function createDropletTrail() {
        const trail = document.createElement('div');
        trail.className = 'water-trail';
        trail.style.left = Math.random() * window.innerWidth + 'px';
        trail.style.animationDuration = (Math.random() * 2 + 3) + 's';
        trail.style.animationDelay = Math.random() * 2 + 's';

        document.body.appendChild(trail);

        // Remove after animation
        setTimeout(() => trail.remove(), 5000);
    }

    // Create occasional trails
    setInterval(createDropletTrail, 3000);

    // CSS for trails
    const trailStyle = document.createElement('style');
    trailStyle.textContent = `
        .water-trail {
            position: fixed;
            top: -20px;
            width: 2px;
            height: 60px;
            background: linear-gradient(180deg, 
                transparent 0%, 
                rgba(129, 212, 250, 0.3) 50%, 
                transparent 100%);
            border-radius: 2px;
            animation: trail-fall linear forwards;
            pointer-events: none;
            z-index: 1;
        }
        
        @keyframes trail-fall {
            0% {
                transform: translateY(0) scaleY(1);
                opacity: 0;
            }
            10% {
                opacity: 1;
            }
            90% {
                opacity: 1;
            }
            100% {
                transform: translateY(100vh) scaleY(1.5);
                opacity: 0;
            }
        }
    `;
    document.head.appendChild(trailStyle);


    // --- SMOOTH SCROLL BEHAVIOR ---
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            const targetId = this.getAttribute('href');
            if (targetId === '#') return;

            const target = document.querySelector(targetId);
            if (target) {
                e.preventDefault();
                target.scrollIntoView({
                    behavior: 'smooth',
                    block: 'start'
                });
            }
        });
    });
});
