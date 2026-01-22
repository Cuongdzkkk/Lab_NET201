// 🌊 NEON ANTIGRAVITY BACKGROUND ANIMATION
// Soft physics-based particle system with neon glow

class NeonParticle {
    constructor(canvas) {
        this.canvas = canvas;
        this.reset();
        this.hue = Math.random() * 60 + 260; // Purple to Cyan range (260-320)
    }

    reset() {
        this.x = Math.random() * this.canvas.width;
        this.y = Math.random() * this.canvas.height;
        this.vx = (Math.random() - 0.5) * 0.5;
        this.vy = (Math.random() - 0.5) * 0.5 - 0.2; // Slight upward bias (antigravity)
        this.radius = Math.random() * 3 + 1;
        this.alpha = Math.random() * 0.5 + 0.3;
    }

    update() {
        // Antigravity effect - particles slowly drift upward
        this.vy -= 0.002;
        
        this.x += this.vx;
        this.y += this.vy;

        // Wrap around edges
        if (this.x < 0) this.x = this.canvas.width;
        if (this.x > this.canvas.width) this.x = 0;
        if (this.y < 0) this.y = this.canvas.height;
        if (this.y > this.canvas.height) this.y = 0;

        // Gentle pulsing alpha
        this.alpha += Math.sin(Date.now() * 0.001) * 0.001;
        this.alpha = Math.max(0.2, Math.min(0.7, this.alpha));
    }

    draw(ctx) {
        // Outer glow
        const gradient = ctx.createRadialGradient(this.x, this.y, 0, this.x, this.y, this.radius * 4);
        gradient.addColorStop(0, `hsla(${this.hue}, 100%, 70%, ${this.alpha * 0.3})`);
        gradient.addColorStop(1, `hsla(${this.hue}, 100%, 70%, 0)`);
        
        ctx.fillStyle = gradient;
        ctx.beginPath();
        ctx.arc(this.x, this.y, this.radius * 4, 0, Math.PI * 2);
        ctx.fill();

        // Core particle
        ctx.fillStyle = `hsla(${this.hue}, 100%, 80%, ${this.alpha})`;
        ctx.beginPath();
        ctx.arc(this.x, this.y, this.radius, 0, Math.PI * 2);
        ctx.fill();
    }
}

class NeonAntigravityBG {
    constructor() {
        this.canvas = document.createElement('canvas');
        this.canvas.id = 'neon-bg-canvas';
        this.canvas.style.position = 'fixed';
        this.canvas.style.top = '0';
        this.canvas.style.left = '0';
        this.canvas.style.width = '100%';
        this.canvas.style.height = '100%';
        this.canvas.style.zIndex = '-1';
        this.canvas.style.pointerEvents = 'none';
        
        document.body.insertBefore(this.canvas, document.body.firstChild);
        
        this.ctx = this.canvas.getContext('2d');
        this.particles = [];
        this.particleCount = 80; // Moderate count for performance
        
        this.resize();
        this.init();
        
        window.addEventListener('resize', () => this.resize());
    }

    resize() {
        this.canvas.width = window.innerWidth;
        this.canvas.height = window.innerHeight;
    }

    init() {
        this.particles = [];
        for (let i = 0; i < this.particleCount; i++) {
            this.particles.push(new NeonParticle(this.canvas));
        }
        this.animate();
    }

    animate() {
        // Fade effect for trails
        this.ctx.fillStyle = 'rgba(10, 10, 20, 0.05)';
        this.ctx.fillRect(0, 0, this.canvas.width, this.canvas.height);

        // Update and draw particles
        this.particles.forEach(particle => {
            particle.update();
            particle.draw(this.ctx);
        });

        requestAnimationFrame(() => this.animate());
    }
}

// Auto-initialize when DOM is ready
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', () => {
        new NeonAntigravityBG();
    });
} else {
    new NeonAntigravityBG();
}
