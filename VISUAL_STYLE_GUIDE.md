# CareSync Visual Style Guide - Quick Reference

## Color Quick Reference

### Primary Slate Palette (Main Brand Colors)
```
████ --primary-50:  #f8fafc  (Background - Lightest)
████ --primary-100: #f1f5f9  (Background - Very Light)
████ --primary-200: #e2e8f0  (Borders - Light)
████ --primary-300: #cbd5e1  (Borders - Medium)
████ --primary-400: #94a3b8  (Icons - Muted)
████ --primary-500: #64748b  (Elements - Medium)
████ --primary-600: #475569  (PRIMARY BRAND - Buttons, Links)
████ --primary-700: #334155  (Emphasis - Dark)
████ --primary-800: #1e293b  (Strong - Very Dark)
████ --primary-900: #0f172a  (Text - Darkest)
```

### Neutral Gray Palette (Supporting Colors)
```
████ --neutral-50:  #fafafa  (Background)
████ --neutral-100: #f5f5f5  (Background)
████ --neutral-200: #e5e5e5  (Borders)
████ --neutral-300: #d4d4d4  (Borders)
████ --neutral-400: #a3a3a3  (Text Muted)
████ --neutral-500: #737373  (Text Secondary)
████ --neutral-600: #525252  (Text Primary)
████ --neutral-700: #404040  (Text Strong)
████ --neutral-800: #262626  (Background Dark)
████ --neutral-900: #171717  (Text Darkest)
```

## Component Color Usage

### Buttons
```css
/* Primary Action */
.btn-primary {
  background: #475569;  /* --primary-600 */
  color: white;
}

/* Secondary Action */
.btn-outline-primary {
  border: #cbd5e1;      /* --primary-300 */
  color: #475569;       /* --primary-600 */
}
```

### Dashboard Cards
```css
.dashboard-card.patients {
  border-top: 3px solid #475569;  /* --primary-600 */
}

.dashboard-card.doctors {
  border-top: 3px solid #334155;  /* --primary-700 */
}

.dashboard-card.appointments {
  border-top: 3px solid #64748b;  /* --primary-500 */
}

.dashboard-card.billing {
  border-top: 3px solid #1e293b;  /* --primary-800 */
}
```

### Status Badges

#### Active/Completed (Dark Backgrounds)
```css
.status-completed {
  background: #0f172a;  /* --primary-900 */
  color: white;
}

.status-paid {
  background: #1e293b;  /* --primary-800 */
  color: white;
}

.status-admitted {
  background: #475569;  /* --primary-600 */
  color: white;
}
```

#### Pending/Scheduled (Light Backgrounds)
```css
.status-scheduled {
  background: #f1f5f9;  /* --primary-100 */
  color: #334155;       /* --primary-700 */
  border: 1px solid #e2e8f0;
}

.status-confirmed {
  background: #f8fafc;  /* --primary-50 */
  color: #1e293b;       /* --primary-800 */
  border: 1px solid #e2e8f0;
}
```

#### Inactive/Cancelled (Gray Backgrounds)
```css
.status-cancelled {
  background: #e5e5e5;  /* --neutral-200 */
  color: #404040;       /* --neutral-700 */
  border: 1px solid #d4d4d4;
}

.status-pending {
  background: #f5f5f5;  /* --neutral-100 */
  color: #404040;       /* --neutral-700 */
  border: 1px solid #e5e5e5;
}
```

### Avatars
```css
/* Default */
.avatar-circle {
  background: #64748b;  /* --primary-500 */
  color: white;
}

/* Primary Variant */
.avatar-circle.bg-primary {
  background: #475569;  /* --primary-600 */
  color: white;
}

/* Light Variant */
.avatar-circle.bg-light {
  background: #f1f5f9;  /* --primary-100 */
  color: #334155;       /* --primary-700 */
}
```

## Typography Scale

```
H1: 30px / 1.875rem     Font Weight: 600    Use: Page Titles
H2: 24px / 1.5rem       Font Weight: 600    Use: Section Headers
H3: 20px / 1.25rem      Font Weight: 600    Use: Subsection Titles
H4: 18px / 1.125rem     Font Weight: 600    Use: Card Headers
H5: 16px / 1rem         Font Weight: 600    Use: Small Headers
H6: 14px / 0.875rem     Font Weight: 600    Use: Minimal Headers
Body: 15px / 0.9375rem  Font Weight: 400    Use: Body Text
Small: 13px / 0.8125rem Font Weight: 400    Use: Helper Text
```

## Spacing Scale

```
Gap 4px   (0.25rem) - Minimal spacing between inline elements
Gap 8px   (0.5rem)  - Small spacing, badges, pills
Gap 16px  (1rem)    - Standard spacing, form fields, card content
Gap 24px  (1.5rem)  - Medium spacing, section gaps
Gap 32px  (2rem)    - Large spacing, page sections
Gap 40px  (2.5rem)  - Extra large, page headers
```

## Border Radius

```
2px  (0.125rem) - Small elements (badges, tags)
4px  (0.25rem)  - Standard (buttons, inputs, small badges)
6px  (0.375rem) - Medium (cards, modals)
8px  (0.5rem)   - Large (containers, panels)
10px (0.625rem) - Extra large (special containers)
999px           - Pills (fully rounded)
```

## Shadow Levels

```
XS:     0 1px 2px 0 rgba(0,0,0,0.05)           - Cards at rest
SM:     0 1px 3px 0 rgba(0,0,0,0.1)            - Small elevation
Base:   0 4px 6px -1px rgba(0,0,0,0.1)         - Hover states
MD:     0 10px 15px -3px rgba(0,0,0,0.1)       - Dropdowns
LG:     0 20px 25px -5px rgba(0,0,0,0.1)       - Modals
XL:     0 25px 50px -12px rgba(0,0,0,0.25)     - Large overlays
```

## Common Patterns

### Card Header
```css
background: #ffffff;
border-bottom: 1px solid #e5e7eb;
padding: 1rem 1.5rem;
font-weight: 600;
font-size: 0.875rem;
color: #171717;
```

### Table Header
```css
background: #fafbfc;
color: #404040;
font-weight: 600;
font-size: 0.8125rem;
padding: 0.875rem 1rem;
border-bottom: 1px solid #e5e7eb;
```

### Input Focus State
```css
border-color: #6b7280;
box-shadow: 0 0 0 3px rgba(107, 114, 128, 0.1);
outline: none;
```

### Button Focus State
```css
outline: 2px solid #475569;
outline-offset: 2px;
```

## Responsive Breakpoints

```
Mobile:         < 576px
Tablet:         576px - 768px
Desktop:        768px - 992px
Large Desktop:  992px - 1200px
XL Desktop:     > 1200px
```

## Icon Sizes

```
Small:    14px / 0.875rem   - Inline with small text
Default:  16px / 1rem       - Inline with body text
Medium:   18px / 1.125rem   - Buttons, navigation
Large:    24px / 1.5rem     - Card headers, features
XL:       32px / 2rem       - Dashboard cards
2XL:      48px / 3rem       - Empty states, heroes
```

## Common Element Heights

```
Input:          36px (0.625rem padding + line-height)
Button:         36px (0.5rem padding + line-height)
Button (small): 28px (0.375rem padding + line-height)
Button (large): 44px (0.75rem padding + line-height)
Nav link:       36px
Table row:      48px (0.875rem padding * 2 + line-height)
```

## Accessibility Requirements

### Text Contrast
```
Primary text on white:     14.8:1 (AAA) ✓
Secondary text on white:    7.8:1 (AAA) ✓
Muted text on white:        4.5:1 (AA)  ✓
Primary button:             7.2:1 (AA)  ✓
```

### Focus Indicators
- All interactive elements must have visible focus
- Focus ring: 2px solid primary color
- Focus offset: 2px
- Never remove focus outline

### Minimum Touch Targets
- Buttons: 36px × 36px minimum
- Links: 36px × 24px minimum
- Icons: 24px × 24px minimum

## Usage Examples

### Page Header
```html
<div class="page-header">
  <h1><i class="fas fa-icon"></i> Page Title</h1>
  <p>Page description text</p>
</div>
```

### Dashboard Card
```html
<div class="dashboard-card patients">
  <i class="fas fa-users fa-3x mb-3"></i>
  <h3>1,234</h3>
  <p>Total Patients</p>
</div>
```

### Status Badge
```html
<span class="status-completed">Completed</span>
<span class="status-pending">Pending</span>
<span class="status-cancelled">Cancelled</span>
```

### Button Group
```html
<div class="btn-group">
  <button class="btn btn-primary">Primary</button>
  <button class="btn btn-outline-primary">Secondary</button>
</div>
```

### Avatar
```html
<div class="avatar-circle">AB</div>
<div class="avatar-circle bg-light">CD</div>
<div class="avatar-circle avatar-lg">EF</div>
```

### Form Input
```html
<label class="form-label">
  <i class="fas fa-icon"></i> Field Label
</label>
<input type="text" class="form-control" placeholder="Enter value">
```

## Do's and Don'ts

### ✅ DO
- Use slate tones for all brand colors
- Maintain consistent spacing throughout
- Follow the typography hierarchy
- Use semantic HTML elements
- Test for accessibility compliance
- Keep shadows subtle
- Use the defined component patterns

### ❌ DON'T
- Introduce colors outside the palette
- Use inline color styles
- Override the design system without documentation
- Create one-off component styles
- Remove focus indicators
- Use decorative fonts
- Mix different spacing scales

## Quick Decision Guide

**Need a button color?** → Use `.btn-primary` (slate)

**Need to show status?** → Use monochromatic status badges

**Need emphasis?** → Use darker slate shade or bold weight

**Need separation?** → Use subtle borders (#e5e7eb) or light background (#f8fafc)

**Need hierarchy?** → Use typography scale + spacing, not color

**Need interactive state?** → Use hover background changes, not color changes

---

**Remember**: This is a monochromatic system. If you think you need a new color, you probably need better spacing, typography, or layout instead.
