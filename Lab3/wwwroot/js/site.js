/* SYSTEM: INTERACTION ENGINE */

$(document).ready(function () {
    // 1. SHUTTER BLINK TRANSITION
    // Default is visible. Only animate OUT on click.
    // Default: Content is visible (scaleY: 0). 
    // We only animate the shutter closed (scaleY: 1) on link clicks.

    // 2. CUSTOM CURSOR LOGIC
    const cursor = document.getElementById('custom-cursor');
    const follower = document.getElementById('cursor-follower');

    if (cursor && follower) {
        document.addEventListener('mousemove', (e) => {
            cursor.style.left = e.clientX + 'px';
            cursor.style.top = e.clientY + 'px';

            // Slight delay for follower
            setTimeout(() => {
                follower.style.left = e.clientX + 'px';
                follower.style.top = e.clientY + 'px';
            }, 50);
        });

        // Hover interactions
        const hoverTargets = 'a, button, input, .product-card, .nav-card';

        $(document).on('mouseenter', hoverTargets, function () {
            $('body').addClass('hover-target');
        });

        $(document).on('mouseleave', hoverTargets, function () {
            $('body').removeClass('hover-target');
        });
    }

    // 3. PAGE TRANSITION HANDLING
    $('a').on('click', function (e) {
        const href = $(this).attr('href');
        // Filter out in-page anchors, mailto, and new tab links
        if (href && href.indexOf('#') !== 0 && !href.startsWith('mailto:') && !$(this).attr('target')) {
            e.preventDefault();
            $('#shutter-blind').addClass('shutter-active'); // Animate shutter CLOSED
            setTimeout(function () {
                window.location = href;
            }, 600); // Wait for animation then navigate
        }
    });
});
