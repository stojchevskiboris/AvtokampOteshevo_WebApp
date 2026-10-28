# Manual Actions & Hosting/CDN Configuration Guide

Some performance optimizations require manual configuration on the web hosting server, IIS, or CDN (Cloudflare / RunASP.net / Azure) dashboard. Follow the instructions below to complete setup.

---

## 1. Configure Static Asset Caching (Cache-Control Header)

### Goal
Resolve the PageSpeed Insights audit: **"Use efficient cache lifetimes"**.

### Steps for IIS / Web.config (RunASP.net / Windows Server):
If hosting on IIS, ensure the following `<staticContent>` configuration is present in the root `Web.config`:

```xml
<configuration>
  <system.webServer>
    <staticContent>
      <clientCache cacheControlCustom="public" cacheControlMode="UseMaxAge" cacheControlMaxAge="365.00:00:00" />
    </staticContent>
  </system.webServer>
</configuration>
```

### Steps for Cloudflare / CDN:
1. Log into your CDN or Cloudflare Dashboard for `otesevo.com`.
2. Go to **Caching** -> **Configuration**.
3. Set **Browser Cache TTL** to **1 year** for static asset extensions (`.webp`, `.jpg`, `.png`, `.css`, `.js`, `.webm`).

---

## 2. Enable Gzip / Brotli Compression

### Goal
Ensure all HTML, CSS, and JS responses are compressed in transit.

### Steps:
1. In IIS / Web Hosting Control Panel, verify that **Dynamic Content Compression** and **Static Content Compression** are enabled.
2. If using Cloudflare, verify **Brotli** compression is turned ON under **Speed** -> **Optimization**.
