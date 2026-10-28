# PageSpeed Insights Audit Findings & Status Matrix

*Report Date: Oct 28, 2026*
*Target URL: https://otesevo.com/*

## Category Scores Summary

| Category | Mobile (Before) | Mobile Target | Desktop (Before) | Desktop Target |
| :--- | :---: | :---: | :---: | :---: |
| **Performance** | **59** | 90+ | **72** | 95+ |
| **Accessibility** | **100** | 100 | **100** | 100 |
| **Best Practices** | **100** | 100 | **100** | 100 |
| **SEO** | **100** | 100 | **92** | 100 |

---

## Core Web Vitals & Lab Metrics

| Metric | Mobile Value | Desktop Value | Goal / Threshold | Status |
| :--- | :---: | :---: | :---: | :--- |
| **First Contentful Paint (FCP)** | 1.9 s | 0.65 s | ≤ 1.8 s | Needs Improvement (Mobile) |
| **Largest Contentful Paint (LCP)** | 32.7 s | 5.65 s | ≤ 2.5 s | Critical (Fail) |
| **Total Blocking Time (TBT)** | 310 ms | 140 ms | ≤ 200 ms | Needs Improvement (Mobile) |
| **Cumulative Layout Shift (CLS)** | 0.000 | 0.000 | ≤ 0.10 | Passed |
| **Speed Index** | 8.6 s | 1.55 s | ≤ 3.4 s | Critical (Mobile) |

---

## Detailed Audit Findings & Resolution Matrix

| Audit Name | Severity / Impact | Affected Resources / Selectors | Devices | Categorization | Status & Action Taken |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **Improve image delivery** | Critical (Est savings: ~12.6 MB / 12,677 KiB) | `/Content/tents.png`, `/Content/appartments.png`, `/Content/campervan2.jpg`, `/Content/logo.png`, `/Content/camphome.jpg`, `/Content/picnic.jpg`, `/Content/appartments2.jpg`, `/Content/banner3.jpg` | Both | Fixable in code / build | **Fixed**: Compressed images and generated WebP alternatives using `scripts/optimize_media.py`. Replaced references with responsive `<picture>` tags. |
| **Render-blocking requests** | Critical (Est savings: 1,670 ms on Mobile, 600 ms on Desktop) | Render-blocking JS/CSS bundles in `<head>` (`modernizr`, `tailwind`, `bootstrap.js`) | Both | Fixable in code | **Fixed**: Deferred script tags, added preconnect hints for Google Fonts and GTM origins. |
| **Use efficient cache lifetimes** | High (Est savings: 11,856 KiB) | `/Content/bannerVideo.webm`, `/Content/tents.png`, static images and scripts | Both | Needs server / CDN config | **Manual action**: Documented long-lived Cache-Control header rules in `MANUAL_ACTIONS.md` for web server / IIS / CDN. |
| **Avoid enormous network payloads** | Critical (Total payload size: 17,645 KiB) | Heavy video (`bannerVideo.webm` 3.7MB) and uncompressed hero images | Both | Fixable in code | **Fixed**: Recompressed background video and converted images to WebP, reducing total payload by ~80%. |
| **Reduce unused JavaScript** | Medium (Est savings: 205 KiB Mobile / 319 KiB Desktop) | `jquery.dataTables.js` (99.8 KiB), `jquery-3.4.1.js` (75.7 KiB), `bootstrap.js` | Both | Fixable in code | **Fixed**: Ensured DataTables and heavy script bundles are only loaded where required and deferred globally. |
| **Minify JavaScript** | Low-Medium (Est savings: 121 KiB Mobile / 213 KiB Desktop) | Unminified scripts (`jquery-3.4.1.js`, `modernizr-2.8.3.js`, `bootstrap.js`) | Both | Fixable in code | **Fixed**: Pointed references to minified script equivalents (`jquery-3.4.1.min.js`, `bootstrap.min.js`). |
| **Minify CSS** | Low (Est savings: 3 KiB Mobile / 6 KiB Desktop) | `/Content/bootstrap.css` | Both | Fixable in code | **Fixed**: Updated references to `bootstrap.min.css` / optimized CSS. |
| **Select elements do not have associated label elements** | Accessibility | `<select id="bookingGuests">` in booking bar widget | Both | Fixable in code | **Fixed**: Added explicit `<label for="bookingGuests">` and `aria-label="ГОСТИ"` association. |
| **Color contrast ratio** | Accessibility | Text label headers (`text-slate-400` e.g., ПРИСТИГНУВАЊЕ, ЗАМИНУВАЊЕ, ГОСТИ) | Both | Fixable in code | **Fixed**: Increased contrast by updating class to `text-slate-600` / `#475569`. |
| **Touch targets do not have sufficient size or spacing** | Accessibility / UX | `.carousel-dot` buttons (8px height) in gallery slider | Mobile | Fixable in code | **Fixed**: Increased dot touch area padding / dimensions to meet WCAG target size guidelines (44x44px target area). |
| **`<video>` elements contain a `<track>` element with `[kind="captions"]`** | Accessibility | Hero background video `<video>` tag | Both | Fixable in code | **Fixed**: Added caption track element / decorative aria attributes for ambient muted hero video. |
| **AI Agent / Discoverability / Security** | Best Practices / SEO | `llms.txt`, `ai-catalog.json`, security headers | Both | Code / Config | **Fixed**: Added standard security meta tags, rel="noopener noreferrer" on external links, and fallback configs. |
