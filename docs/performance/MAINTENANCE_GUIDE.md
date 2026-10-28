# Performance Maintenance & Developer Guide

## Overview
This guide provides developers and maintainers with the necessary procedures, tools, and guidelines to ensure https://otesevo.com maintains high performance, accessibility, SEO, and security standards over time.

---

## 1. Do's and Don'ts for Performance Maintenance

### Media & Assets
- **DO** convert all newly added static images to WebP format using `python3 scripts/optimize_media.py` before committing.
- **DO** use the `<picture>` element with `<source type="image/webp">` and PNG/JPG fallbacks in Razor views.
- **DO** set explicit `width` and `height` attributes on image elements to prevent Cumulative Layout Shift (CLS).
- **DON'T** use uncompressed PNGs or raw camera JPEGs directly in views.

### Scripts & Third Parties
- **DO** load non-critical JavaScript asynchronously or with `defer`.
- **DO** add `rel="noopener noreferrer"` to any external links opening in a new tab (`target="_blank"`).
- **DON'T** add blocking synchronous third-party scripts in the `<head>` section.

---

## 2. Running Optimization Tools & Scripts

### Image & Video Optimization Script
To automatically convert newly added images in `Project_IT/Content/` to WebP and compress hero videos, run:
```bash
python3 scripts/optimize_media.py
```

### Registering New Assets in Visual Studio
When adding new `.webp` or compressed images to `Project_IT/Content/`, make sure they are referenced in `Project_IT/Project_IT.csproj`:
```xml
<Content Include="Content\your-new-image.webp" />
```

---

## 3. Lighthouse & PageSpeed Audits

### Re-running PageSpeed Insights
To re-evaluate performance scores post-deployment:
1. Visit [PageSpeed Insights](https://pagespeed.web.dev/).
2. Enter `https://otesevo.com/mk/`.
3. Analyze both Mobile and Desktop tab reports.

---

## 4. Performance Budgets & CI Thresholds
The repository includes a `lighthouserc.json` file defining minimum acceptable scores:
- **Performance:** ≥ 0.90
- **Accessibility:** 1.00
- **Best Practices:** 1.00
- **SEO:** 1.00
