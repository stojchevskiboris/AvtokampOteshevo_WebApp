# Performance & Accessibility Changelog

All notable changes made to optimize https://otesevo.com performance, accessibility, SEO, and security best practices.

## [1.0.0] - 2026-10-28

### Performance Optimizations
- **Image Conversion to WebP:** Converted all PNG/JPG assets in `Project_IT/Content/` to high-efficiency WebP format using `scripts/optimize_media.py`.
- **Responsive Markup & Fallbacks:** Updated Razor views (`_Layout.cshtml`, `Home/Index.cshtml`) with HTML5 `<picture>` elements to serve `.webp` images with fallback JPEG/PNG support.
- **LCP Optimization:** Applied `fetchpriority="high"` and eager loading to the Largest Contentful Paint image; added `loading="lazy"` and `decoding="async"` to all below-the-fold media elements.
- **Hero Video Compression:** Recompressed ambient background video (`bannerVideo.webm` / `bannerVideo.mp4`) using FFmpeg (VP9/H.264), reducing total file payload from ~7.3MB to ~1.4MB (over 80% compression).
- **Script Deferral:** Added `defer` attributes to non-critical JavaScript files (`tailwind_3-4-17.min.js`, `alpinejs.min.js`).
- **Resource Hints:** Injected `<link rel="preconnect">` and `<link rel="dns-prefetch">` tags for Google Fonts and Google Tag Manager domains in `_Layout.cshtml`.

### Accessibility Improvements (WCAG 2.2 AA)
- **Color Contrast:** Increased text contrast on booking bar label headers from `text-slate-400` to `text-slate-600`.
- **Form Control Association:** Associated `<select id="bookingGuests">` with explicit `<label>` markup and `aria-label`.
- **Touch Targets:** Expanded touch target hit area padding on carousel slider dot indicators (`.carousel-dot`) to achieve a minimum 44x44 CSS px target size.
- **Video Accessibility:** Injected caption track placeholder `<track kind="captions">` and decorative `aria-hidden="true"` attributes to ambient hero video.

### Security & SEO Best Practices
- Injected `rel="noopener noreferrer"` across all external `target="_blank"` anchor tags.
- Added strict referrer policy `<meta name="referrer" content="strict-origin-when-cross-origin">` in layout template head.

### Maintainability & Automation
- Created `scripts/optimize_media.py` for automated image and video compression.
- Created `lighthouserc.json` establishing Lighthouse CI performance budgets.
- Updated `Project_IT/Project_IT.csproj` with entries for newly generated `.webp` static assets.
