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

    // 4. NEURAL MONITOR SYSTEM (Oscilloscope & Live Data)
    const monitorCanvas = document.getElementById('neural-monitor');

    if (monitorCanvas) {
        const ctx = monitorCanvas.getContext('2d');
        let width, height;
        let pData = [];
        let hue = 180; // Cyan base
        let isAlert = false;

        // Resize handler
        const resize = () => {
            width = monitorCanvas.width = monitorCanvas.offsetWidth;
            height = monitorCanvas.height = monitorCanvas.offsetHeight;
        };
        window.addEventListener('resize', resize);
        resize();

        // Wave Animation Loop
        const drawWave = () => {
            ctx.clearRect(0, 0, width, height);

            // Dynamic Color
            const color = isAlert ? 'rgba(255, 0, 60, 0.5)' : 'rgba(0, 243, 255, 0.3)';
            const lineWidth = isAlert ? 3 : 2;

            ctx.beginPath();
            ctx.lineWidth = lineWidth;
            ctx.strokeStyle = color;
            ctx.shadowBlur = 10;
            ctx.shadowColor = isAlert ? '#ff003c' : '#00f3ff';

            // Generate random wave if no fresh data, or smooth interpolation
            // For visual effect, we use a sine wave + noise
            const time = Date.now() * 0.002;

            for (let x = 0; x < width; x++) {
                // Sine wave synthesis
                const y = Math.sin(x * 0.02 + time) * 20 +
                    Math.sin(x * 0.05 + time * 2) * 10 +
                    (Math.random() - 0.5) * 5; // Noise

                // Center vertically
                ctx.lineTo(x, height / 2 + y);
            }

            ctx.stroke();
            requestAnimationFrame(drawWave);
        };
        drawWave();

        // Silent Fetch (Live Data)
        const fetchData = async () => {
            try {
                const response = await fetch('/Home/GetSystemMetrics');
                if (!response.ok) return;
                const data = await response.json();

                // Update DOM
                document.getElementById('metric-cpu').innerText = `${data.cpu}%`;
                document.getElementById('metric-ram').innerText = `${data.ram}GB`;
                document.getElementById('metric-temp').innerText = `${data.temp}°C`;

                // Alert State Logic
                isAlert = data.cpu > 80;
                const indicator = document.getElementById('status-indicator');
                if (isAlert) {
                    indicator.classList.remove('bg-cyan-400', 'shadow-[0_0_10px_#00f3ff]');
                    indicator.classList.add('bg-red-500', 'shadow-[0_0_10px_#ff003c]');
                    document.getElementById('metric-cpu').classList.add('text-red-500');
                    document.getElementById('metric-cpu').classList.remove('text-cyan-400');
                } else {
                    indicator.classList.add('bg-cyan-400', 'shadow-[0_0_10px_#00f3ff]');
                    indicator.classList.remove('bg-red-500', 'shadow-[0_0_10px_#ff003c]');
                    document.getElementById('metric-cpu').classList.remove('text-red-500');
                    document.getElementById('metric-cpu').classList.add('text-cyan-400');
                }

            } catch (err) { console.error('Neural Link Lost', err); }
        };
        setInterval(fetchData, 2000);
        fetchData(); // Initial call
    }

    // 5. Magnetic Dock Buttons
    const dockItems = document.querySelectorAll('.dock-item');
    dockItems.forEach(item => {
        item.addEventListener('mousemove', (e) => {
            const rect = item.getBoundingClientRect();
            const x = e.clientX - rect.left - rect.width / 2;
            const y = e.clientY - rect.top - rect.height / 2;

            // Magnetic pull strength
            item.style.transform = `translate(${x * 0.3}px, ${y * 0.3}px) scale(1.1)`;
        });

        item.addEventListener('mouseleave', () => {
            item.style.transform = 'translate(0, 0) scale(1)';
        });
    });

    // 6. Scanline Trigger (Static only)
    document.body.classList.add('scanline');

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

    // 7. SECURITY TERMINAL LOGIC
    const terminalInput = document.getElementById('terminal-input');
    const terminalOutput = document.getElementById('terminal-output');
    const btnLogin = document.getElementById('btn-login');
    const btnProtocol = document.getElementById('btn-protocol');
    const securityScreen = document.getElementById('security-screen');

    // Typewriter Effect Function
    const typeWriter = (text, element, speed = 30) => {
        element.innerHTML = ''; // Clear previous
        let i = 0;
        const type = () => {
            if (i < text.length) {
                element.innerHTML += text.charAt(i);
                i++;
                setTimeout(type, speed);
            }
        };
        type();
    };

    if (btnLogin) {
        btnLogin.addEventListener('click', async () => {
            const code = terminalInput.value;
            terminalOutput.innerHTML = '<span class="text-green-500 animate-pulse">AUTHENTICATING...</span>';

            // Slight delay for realism
            setTimeout(async () => {
                try {
                    // Call Backend: Authenticate (POST)
                    const formData = new FormData();
                    formData.append('code', code);

                    const response = await fetch('/Security/Authenticate', {
                        method: 'POST',
                        body: formData
                    });

                    const resultText = await response.text();

                    if (resultText.includes("DENIED")) {
                        // ERROR EFFECT
                        securityScreen.parentElement.classList.add('animate-shake');
                        terminalOutput.classList.add('text-red-500');
                        typeWriter(resultText, terminalOutput);

                        setTimeout(() => {
                            securityScreen.parentElement.classList.remove('animate-shake');
                            terminalOutput.classList.remove('text-red-500');
                        }, 500);
                    } else {
                        // SUCCESS EFFECT
                        terminalOutput.classList.add('text-green-400');
                        typeWriter(resultText, terminalOutput);
                    }

                } catch (err) {
                    terminalOutput.innerText = "CONNECTION_ERROR::" + err;
                }
            }, 800);
        });
    }

    if (btnProtocol) {
        btnProtocol.addEventListener('click', async () => {
            terminalOutput.innerHTML = '<span class="text-yellow-500">INITIALIZING PROTOCOL...</span>';

            try {
                // Call Backend: ExecuteOrder66 (ActionName Demo)
                const response = await fetch('/Security/ExecuteOrder66');
                const text = await response.text();

                typeWriter(text, terminalOutput, 50);

            } catch (err) {
                terminalOutput.innerText = "PROTOCOL_FAILURE::" + err;
            }
        });
    }

});

// --- GLOBAL DIAGNOSTIC FUNCTIONS (Called via OnClick) ---

window.runDiagnostic = async (type) => {
    const diagModal = document.getElementById('diag-modal');
    const diagBody = document.getElementById('diag-body');
    if (!diagModal || !diagBody) return;

    if (type === 'redirect') {
        // Show Loading Spin then Redirect
        const loader = document.getElementById('nexus-loader');
        if (loader) {
            loader.classList.remove('opacity-0');
            loader.innerHTML = '<div class="cube-loader"></div><div class="text-white font-mono mt-4">REROUTING...</div>';
            setTimeout(() => {
                window.location.href = '/Calibration/TestRedirect';
            }, 1000);
        }
        return;
    }

    // Open Modal State
    diagModal.classList.remove('opacity-0', 'pointer-events-none');
    diagModal.querySelector('#diag-modal-content').classList.remove('scale-95');
    diagModal.querySelector('#diag-modal-content').classList.add('scale-100');

    diagBody.innerHTML = '<div class="animate-pulse text-cyan-500">ACCESSING CORE DATABANKS...</div>';

    try {
        let url = '';
        if (type === 'content') url = '/Calibration/TestContent';
        if (type === 'json') url = '/Calibration/TestJson';

        const res = await fetch(url);

        if (type === 'json') {
            const data = await res.json();
            diagBody.innerHTML = `<pre class="text-xs text-green-300 whitespace-pre-wrap">${JSON.stringify(data, null, 2)}</pre>`;
        } else {
            const text = await res.text();
            diagBody.innerHTML = `<div class="text-cyan-300 border-l-2 border-cyan-500 pl-3">${text}</div>`;
        }

    } catch (err) {
        diagBody.innerHTML = `<div class="text-red-500">DIAGNOSTIC FAILURE: ${err}</div>`;
    }
};

window.closeModal = () => {
    const diagModal = document.getElementById('diag-modal');
    if (!diagModal) return;
    diagModal.classList.add('opacity-0', 'pointer-events-none');
    diagModal.querySelector('#diag-modal-content').classList.add('scale-95');
    diagModal.querySelector('#diag-modal-content').classList.remove('scale-100');
};
