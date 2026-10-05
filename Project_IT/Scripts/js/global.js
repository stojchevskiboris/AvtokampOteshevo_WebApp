document.addEventListener('DOMContentLoaded', function () {
    // Mobile Menu Toggle
    var mobileMenu = document.getElementById('mobileMenu');
    var mobileMenuBtn = document.getElementById('mobileMenuBtn');
    var mobileMenuIsAnimating = false;

    // Mobile Menu Overlay
    var mobileMenuOverlay = document.createElement('div');
    mobileMenuOverlay.id = 'mobileMenuOverlay';
    mobileMenuOverlay.className = 'hidden fixed inset-0 bg-black/40 z-40 sm:hidden';
    document.body.appendChild(mobileMenuOverlay);

    function openMobileMenu() {
        if (!mobileMenu || mobileMenuIsAnimating) return;

        mobileMenu.classList.remove('hidden', 'mobile-menu-anim-out');
        mobileMenu.classList.add('mobile-menu-anim-in');

        mobileMenuOverlay.classList.remove('hidden');
        document.body.classList.add('overflow-hidden');
    }

    function closeMobileMenu() {
        if (!mobileMenu || mobileMenu.classList.contains('hidden') || mobileMenuIsAnimating) return;

        mobileMenuIsAnimating = true;
        mobileMenu.classList.remove('mobile-menu-anim-in');
        mobileMenu.classList.add('mobile-menu-anim-out');

        mobileMenuOverlay.classList.add('hidden');
        document.body.classList.remove('overflow-hidden');

        setTimeout(function () {
            mobileMenu.classList.add('hidden');
            mobileMenu.classList.remove('mobile-menu-anim-out');
            mobileMenuIsAnimating = false;
        }, 180);
    }

    if (mobileMenuBtn && mobileMenu) {
        mobileMenuBtn.addEventListener('click', function () {
            if (mobileMenu.classList.contains('hidden')) {
                openMobileMenu();
            } else {
                closeMobileMenu();
            }
        });

        // Close Mobile Menu when any link inside it is clicked
        var mobileMenuLinks = mobileMenu.querySelectorAll('a');
        mobileMenuLinks.forEach(function (link) {
            link.addEventListener('click', function () {
                closeMobileMenu();
            });
        });

        // Close Mobile Menu when clicking the overlay
        mobileMenuOverlay.addEventListener('click', function () {
            closeMobileMenu();
        });

        // Close Mobile Menu with Escape
        document.addEventListener('keydown', function (e) {
            if (e.key === 'Escape') {
                closeMobileMenu();
            }
        });

        // Close Mobile Menu when switching to desktop resolution
        window.addEventListener('resize', function () {
            if (window.innerWidth >= 640 && !mobileMenu.classList.contains('hidden')) {
                mobileMenu.classList.add('hidden');
                mobileMenu.classList.remove('mobile-menu-anim-in', 'mobile-menu-anim-out');
                mobileMenuOverlay.classList.add('hidden');
                document.body.classList.remove('overflow-hidden');
                mobileMenuIsAnimating = false;
            }
        });
    }

    // Desktop Admin Dropdown Toggle
    var adminBtn = document.getElementById('adminDropdownBtn');
    var adminMenu = document.getElementById('adminDropdownMenu');

    if (adminBtn && adminMenu) {
        adminBtn.addEventListener('click', function (e) {
            e.stopPropagation();
            adminMenu.classList.toggle('hidden');
        });

        document.addEventListener('click', function (e) {
            if (!adminMenu.contains(e.target) && !adminBtn.contains(e.target)) {
                adminMenu.classList.add('hidden');
            }
        });
    }

    // Sticky Header scroll styling
    window.addEventListener('scroll', function () {
        var header = document.getElementById('mainHeader');
        if (header) {
            if (window.scrollY > 40) {
                header.classList.add('shadow-md');
            } else {
                header.classList.remove('shadow-md');
            }
        }
    });
});