# CareSync Design System - Quick Reference

## 🎨 Color Palette

### Primary Colors
```css
Primary: #475569 (Slate 600)
Primary Hover: #334155 (Slate 700)
Primary Light: #f8fafc (Slate 50)
Primary Border: #cbd5e1 (Slate 300)
```

### Semantic Colors
```css
Success: #10b981 (Emerald 500)
Warning: #f59e0b (Amber 500)
Danger: #ef4444 (Red 500)
Info: #3b82f6 (Blue 500)
```

### Neutral Grays
```css
Background: #fafbfc
Card Background: #ffffff
Border: #e5e7eb
Border Secondary: #d1d5db
Text Primary: #171717 (Neutral 900)
Text Secondary: #525252 (Neutral 600)
Text Tertiary: #737373 (Neutral 500)
Text Muted: #9ca3af (Gray 400)
```

### Sidebar
```css
Background: #1e293b (Slate 800)
Active Border: #64748b (Slate 500)
```

---

## 📝 Typography

### Font Family
```css
font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', system-ui, sans-serif;
```

### Font Sizes
```css
Body: 0.9375rem (15px)
Small: 0.8125rem (13px)
Button: 0.875rem (14px)

h1: 1.875rem (30px)
h2: 1.5rem (24px)
h3: 1.25rem (20px)
h4: 1.125rem (18px)
h5: 1rem (16px)
h6: 0.875rem (14px)
```

### Font Weights
```css
Normal: 400
Medium: 500
Semibold: 600
```

### Letter Spacing
```css
body: -0.01em
headings: -0.02em
```

### Line Heights
```css
Body: 1.65
Headings: 1.4
Tight: 1.5
```

---

## 📏 Spacing Scale

### Padding
```css
xs: 0.375rem (6px)
sm: 0.5rem (8px)
md: 0.75rem (12px)
base: 1rem (16px)
lg: 1.5rem (24px)
xl: 2rem (32px)
2xl: 2.5rem (40px)
```

### Margins
```css
Card bottom: 2rem
Section: 2.5rem
Page header bottom: 2.5rem
```

---

## 🔲 Border Radius

```css
sm: 2px
base: 4px
md: 6px
lg: 8px
xl: 10px
full: 9999px (rounded-full)
```

---

## 🎭 Shadows

```css
Subtle: 0 1px 2px 0 rgba(0, 0, 0, 0.05)
Small: 0 4px 6px -1px rgba(0, 0, 0, 0.08)
Medium: 0 10px 15px -3px rgba(0, 0, 0, 0.1)
Large: 0 20px 25px -5px rgba(0, 0, 0, 0.1)
```

---

## 🔘 Buttons

### Primary Button
```css
background: #475569
color: #ffffff
padding: 0.5rem 1rem
border-radius: 4px
font-size: 0.875rem
font-weight: 500
transition: all 0.1s ease
```

### Hover State
```css
background: #334155
```

### Outline Button
```css
background: transparent
color: #475569
border: 1px solid #cbd5e1
```

### Button Sizes
```css
Small: padding 0.375rem 0.625rem, font-size 0.75rem
Default: padding 0.5rem 1rem, font-size 0.875rem
Large: padding 0.75rem 1.5rem, font-size 1.1rem
```

---

## 📋 Cards

```css
background: #ffffff
border: 1px solid #e5e7eb
border-radius: 6px
box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.05)
margin-bottom: 2rem
```

### Card Header
```css
background: #ffffff
border-bottom: 1px solid #e5e7eb
padding: 1rem 1.5rem
font-size: 0.875rem
font-weight: 600
```

### Card Body
```css
padding: 1.5rem
```

---

## 📊 Tables

### Header
```css
background: #fafbfc
color: #374151
font-size: 0.8125rem
font-weight: 600
padding: 0.875rem 1rem
```

### Body
```css
font-size: 0.875rem
padding: 0.875rem 1rem
```

### Hover State
```css
background: #f9fafb
```

---

## 🏷️ Status Badges

```css
font-size: 0.75rem
font-weight: 500
padding: 0.375rem 0.75rem
border-radius: 4px
border: 1px solid
```

### Examples
```css
Success:
  background: #f0fdf4
  color: #15803d
  border-color: #dcfce7

Warning:
  background: #fffbeb
  color: #b45309
  border-color: #fef3c7

Danger:
  background: #fef2f2
  color: #b91c1c
  border-color: #fee2e2
```

---

## 📝 Forms

### Input Fields
```css
border: 1px solid #d1d5db
border-radius: 4px
padding: 0.625rem 0.875rem
font-size: 0.875rem
background: #ffffff
```

### Focus State
```css
border-color: #6b7280
box-shadow: 0 0 0 3px rgba(107, 114, 128, 0.1)
```

### Labels
```css
font-size: 0.875rem
font-weight: 500
color: #374151
margin-bottom: 0.5rem
```

---

## 🔍 Search Box

```css
background: #ffffff
border: 1px solid #e5e7eb
border-radius: 6px
padding: 1rem 1.25rem
```

---

## 📑 Pagination

```css
min-width: 36px
height: 36px
font-size: 0.875rem
border: 1px solid #d1d5db
border-radius: 4px
background: #ffffff
```

### Active State
```css
background: #475569
color: #ffffff
border-color: #475569
```

---

## 🎯 Dashboard Cards

```css
background: #ffffff
border: 1px solid #e5e7eb
border-radius: 6px
padding: 2rem 1.5rem
text-align: left
min-height: 140px
```

### Number Display
```css
font-size: 2rem
font-weight: 600
color: #171717
letter-spacing: -0.03em
```

### Label
```css
font-size: 0.8125rem
font-weight: 500
color: #525252
```

### Color Accents
```css
Patients: border-top 3px solid #3b82f6
Doctors: border-top 3px solid #10b981
Appointments: border-top 3px solid #f59e0b
Billing: border-top 3px solid #ef4444
```

---

## 🎨 Sidebar Navigation

### Sidebar
```css
width: 260px
background: #1e293b
```

### Nav Link
```css
padding: 0.75rem 1.5rem
font-size: 0.875rem
font-weight: 500
color: rgba(255, 255, 255, 0.7)
```

### Active State
```css
background: rgba(255, 255, 255, 0.08)
color: #ffffff
border-left: 3px solid #64748b
```

---

## ⚡ Animations & Transitions

### Philosophy
- Minimal animations
- Fast transitions (0.1s ease)
- No transforms on hover
- Only essential loading states

### Allowed
```css
transition: all 0.1s ease (for hover states)
transition: background-color 0.1s ease
transition: box-shadow 0.15s ease
```

### Forbidden
```css
❌ transform: translateY()
❌ transform: scale()
❌ Complex cubic-bezier
❌ Decorative animations
```

---

## 📱 Responsive Breakpoints

```css
Mobile: max-width 576px
Tablet: max-width 768px
Desktop: min-width 992px
```

---

## ✅ Best Practices

1. **Always use rem** for sizing (except borders use px)
2. **Avoid transforms** on hover
3. **Keep transitions fast** (0.1-0.15s)
4. **Use subtle shadows** (0-2px offset)
5. **Maintain consistent spacing** (use spacing scale)
6. **Prefer flat design** over gradients
7. **Use semantic colors** for status
8. **Keep font weights moderate** (400-600)
9. **Maintain high contrast** for accessibility
10. **Test with real data** not lorem ipsum

---

## 🚫 Things to Avoid

- ❌ Bright, saturated colors
- ❌ Large border radius (>10px)
- ❌ Heavy shadows
- ❌ Uppercase text everywhere
- ❌ Too many font weights
- ❌ Decorative animations
- ❌ Gradients on everything
- ❌ Pure black backgrounds
- ❌ Small touch targets (<36px)
- ❌ Low contrast text

---

## 🎯 Design Goals

1. **Professional**: Medical-grade appearance
2. **Serious**: No playful or trendy elements
3. **Clean**: Minimal visual noise
4. **Efficient**: Fast, distraction-free
5. **Trustworthy**: Reliable, stable aesthetic
6. **Accessible**: High contrast, clear hierarchy
7. **Timeless**: Won't feel dated quickly
8. **Focused**: Data-first approach

---

*This design system prioritizes clarity, professionalism, and efficiency over decoration and animation.*
