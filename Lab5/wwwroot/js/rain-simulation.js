/* ============================================
   NATURAL RAIN SIMULATION - Subtle & Elegant
   Max 100 particles, 20% opacity, soft blue
   ============================================ */

const canvas = document.createElement('canvas');
const ctx = canvas.getContext('2d');
const container = document.getElementById('canvas-container');

if (container) {
    container.appendChild(canvas);

    let width, height;
    let drops = [];
    let splashes = [];
    const MAX_DROPS = 100; // Performance cap

    function resize() {
        width = window.innerWidth;
        height = window.innerHeight;
        canvas.width = width;
        canvas.height = height;
    }

    window.addEventListener('resize', resize);
    resize();

    // Drop class - individual raindrop
    class Drop {
        constructor() {
            this.init();
        }

        init() {
            this.x = Math.random() * width;
            this.y = Math.random() * -height;
            this.vy = Math.random() * 6 + 8; // Slower, more gentle
            this.l = Math.random() * 15 + 10; // Length
            this.opacity = Math.random() * 0.15 + 0.05; // Very subtle (5-20%)
        }

        update() {
            this.y += this.vy;
            if (this.y > height) {
                // Small chance to create splash
                if (Math.random() > 0.7 && splashes.length < 30) {
                    for (let i = 0; i < 2; i++) {
                        splashes.push(new Splash(this.x, height));
                    }
                }
                this.init();
            }
        }

        draw() {
            ctx.beginPath();
            // Soft blue color
            ctx.strokeStyle = `rgba(129, 212, 250, ${this.opacity})`;
            ctx.lineWidth = 1.5;
            ctx.lineCap = 'round';
            ctx.moveTo(this.x, this.y);
            ctx.lineTo(this.x, this.y + this.l);
            ctx.stroke();
        }
    }

    // Splash class - water impact particles
    class Splash {
        constructor(x, y) {
            this.x = x;
            this.y = y;
            this.vx = (Math.random() - 0.5) * 3;
            this.vy = Math.random() * -2 - 1;
            this.life = 1.0;
            this.maxLife = 1.0;
        }

        update() {
            this.x += this.vx;
            this.y += this.vy;
            this.vy += 0.15; // Gravity
            this.life -= 0.04;
        }

        draw() {
            const opacity = this.life * 0.15; // Very subtle splashes
            ctx.beginPath();
            ctx.fillStyle = `rgba(129, 212, 250, ${opacity})`;
            ctx.arc(this.x, this.y, 1.5, 0, Math.PI * 2);
            ctx.fill();
        }
    }

    // Initialize drops
    for (let i = 0; i < MAX_DROPS; i++) {
        drops.push(new Drop());
    }

    // Animation loop
    function animate() {
        // Clear with subtle gradient background
        ctx.clearRect(0, 0, width, height);

        // Update and draw drops
        drops.forEach(drop => {
            drop.update();
            drop.draw();
        });

        // Update and draw splashes
        for (let i = splashes.length - 1; i >= 0; i--) {
            const splash = splashes[i];
            splash.update();
            splash.draw();
            if (splash.life <= 0) {
                splashes.splice(i, 1);
            }
        }

        requestAnimationFrame(animate);
    }

    // Start with fade-in
    canvas.style.opacity = '0';
    canvas.style.transition = 'opacity 0.5s ease-in';

    setTimeout(() => {
        canvas.style.opacity = '1';
        animate();
    }, 100);
}
