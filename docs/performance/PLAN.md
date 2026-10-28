# Performance, Accessibility, Best Practices & SEO Engineering Plan

## Executive Overview
This document outlines the systematic engineering strategy implemented to optimize **https://otesevo.com** for maximum Lighthouse category scores across Mobile and Desktop environments.

---

## Technical Strategy & Execution Phases

### Phase 1: Media & Payload Compression
- **Images:**
  - Create a Python script (`scripts/optimize_media.py`) utilizing `Pillow` to convert PNG/JPG assets in `Project_IT/Content/` to WebP format.
  - Implement responsive `<picture>` markup in Razor views with `<source type="image/webp">` and fallbacks.
  - Apply `fetchpriority="high"` and eager loading for above-the-fold / LCP elements; apply `loading="lazy"` and `decoding="async"` to all below-the-fold images.
- **Video:**
  - Recompress ambient hero background video (`bannerVideo.webm` / `bannerVideo.mp4`) to minimize footprint without visual degradation.

### Phase 2: Loading & Script Optimization
- **Script Deferral:**
  - Add `defer` attributes to non-critical scripts (`modernizr`, `tailwind_3-4-17.min.js`, `alpinejs.min.js`, DataTables).
  - Use minified versions of core dependencies (`jquery-3.4.1.min.js`, `bootstrap.min.js`, `bootstrap.min.css`).
- **Resource Hints:**
  - Add `preconnect` and `dns-prefetch` links for third-party origins:
    - `https://fonts.googleapis.com`
    - `https://fonts.gstatic.com`
    - `https://www.googletagmanager.com`

### Phase 3: Accessibility (WCAG 2.2 AA) Compliance
- **Contrast Ratios:**
  - Adjust muted text elements (`text-slate-400`) in the booking bar widget to `text-slate-600` to satisfy the minimum 4.5:1 contrast requirement.
- **Form Control Associations:**
  - Bind `<select id="bookingGuests">` with explicit `<label>` markup and `aria-label`.
- **Touch Target Sizes:**
  - Expand carousel indicator dots (`.carousel-dot`) touch area padding to meet the minimum 44x44 CSS px requirement.
- **Captions / Video Accessibility:**
  - Add appropriate caption tracks or accessibility attributes to muted autoplaying video.

### Phase 4: Best Practices & Technical SEO
- **Security Attributes:**
  - Ensure all external `target="_blank"` links include `rel="noopener noreferrer"`.
- **Performance Budgets & CI:**
  - Establish `lighthouserc.json` configuration defining performance budgets and score thresholds for CI runs.

---

## Verification & Rollout Plan
1. Re-build and verify file assets in `Project_IT.csproj`.
2. Inspect rendered markup across main pages and views for zero console errors or visual regressions.
3. Document manual actions required for hosting/CDN server cache control headers in `MANUAL_ACTIONS.md`.
