# CareSync Icon Usage Guide - Quick Reference

## 🚀 Quick Start

### DO ✅
```html
<!-- Inline with text -->
<i class="fas fa-user me-2"></i> Patient Name

<!-- Section headers -->
<h6><i class="fas fa-icon me-2"></i>Section Title</h6>

<!-- Dashboard cards -->
<i class="fas fa-users fa-3x mb-3"></i>

<!-- Quick actions -->
<i class="fas fa-plus fa-2x mb-2"></i>

<!-- Empty states (muted acceptable) -->
<i class="fas fa-folder fa-3x mb-3 text-muted"></i>
```

### DON'T ❌
```html
<!-- NO color classes (breaks monochromatic design) -->
<i class="fas fa-user text-primary"></i> ❌
<i class="fas fa-user-md text-success"></i> ❌
<i class="fas fa-icon text-danger"></i> ❌
<i class="fas fa-icon text-warning"></i> ❌
<i class="fas fa-icon text-info"></i> ❌

<!-- NO inconsistent spacing -->
<i class="fas fa-icon me-4"></i> ❌
<i class="fas fa-icon ms-2"></i> ❌
```

## 📐 Standard Patterns

### By Context

```html
<!-- Navigation Links -->
<i class="fas fa-tachometer-alt me-2"></i> Dashboard

<!-- Page Headers -->
<h1><i class="fas fa-users"></i> Patients</h1>

<!-- Dashboard Statistics Cards -->
<div class="dashboard-card">
    <i class="fas fa-users fa-3x mb-3"></i>
    <h3>1,234</h3>
    <p>Total Patients</p>
</div>

<!-- Quick Action Buttons -->
<a href="#" class="btn btn-outline-primary">
    <i class="fas fa-user-plus fa-2x mb-2"></i>
    <span>Add Patient</span>
</a>

<!-- Card/Section Headers -->
<div class="card-header">
    <i class="fas fa-list me-2"></i>Recent Activity
</div>

<!-- Table Rows (inline with data) -->
<td>
    <i class="fas fa-user me-2"></i>
    @patient.Name
</td>

<!-- Action Buttons (icon only) -->
<button class="btn btn-sm btn-outline-primary">
    <i class="fas fa-edit"></i>
</button>

<!-- Empty States -->
<div class="empty-state">
    <i class="fas fa-inbox fa-3x mb-3 text-muted"></i>
    <p>No items found</p>
</div>

<!-- Modal Headers -->
<div class="modal-header">
    <h5 class="modal-title">
        <i class="fas fa-file-invoice me-2"></i>Invoice Details
    </h5>
</div>
```

## 📏 Size Guidelines

| Size Class | Font Size | Use For |
|------------|-----------|---------|
| (default) | 1em | Navigation, buttons, headers, inline text |
| `fa-lg` | 1.33em | Slightly larger inline icons (rare) |
| `fa-2x` | 2em | Quick actions, featured buttons |
| `fa-3x` | 3em | Dashboard cards, empty states, hero icons |
| `fa-4x` | 4em | Large illustrations, splash screens |

## 📏 Spacing Guidelines

| Spacing | Value | Use For |
|---------|-------|---------|
| `me-2` | 0.5rem | Inline icons (standard) |
| `me-3` | 0.75rem | Avatars with larger spacing |
| `mb-2` | 0.5rem | Stacked icons (fa-2x) |
| `mb-3` | 0.75rem | Stacked icons (fa-3x) |

## 🎨 Color Guidelines

### Allowed
```html
<!-- NO COLOR CLASS - Inherits from parent -->
<i class="fas fa-icon"></i>

<!-- MUTED - Only for empty/disabled states -->
<i class="fas fa-icon text-muted"></i>

<!-- OPACITY - Only for illustrations -->
<i class="fas fa-icon opacity-50"></i>
```

### NEVER USE
```html
<i class="fas fa-icon text-primary"></i> ❌
<i class="fas fa-icon text-success"></i> ❌
<i class="fas fa-icon text-danger"></i> ❌
<i class="fas fa-icon text-warning"></i> ❌
<i class="fas fa-icon text-info"></i> ❌
<i class="fas fa-icon text-dark"></i> ❌
<i class="fas fa-icon text-light"></i> ❌
```

## 🔍 Common Patterns by Page

### Dashboard (Home.razor)
```html
<!-- Stats Cards -->
<i class="fas fa-users fa-3x mb-3"></i>
<i class="fas fa-user-md fa-3x mb-3"></i>
<i class="fas fa-calendar-alt fa-3x mb-3"></i>
<i class="fas fa-dollar-sign fa-3x mb-3"></i>

<!-- Quick Actions -->
<i class="fas fa-user-plus fa-2x mb-2"></i>
<i class="fas fa-calendar-plus fa-2x mb-2"></i>

<!-- Section Headers -->
<i class="fas fa-bolt me-2"></i>Quick Actions
<i class="fas fa-clock me-2"></i>Recent Appointments
```

### List Pages (Patients, Doctors, etc.)
```html
<!-- Table inline icons -->
<i class="fas fa-user me-2"></i>
<i class="fas fa-user-md me-2"></i>

<!-- Action buttons -->
<i class="fas fa-edit"></i>
<i class="fas fa-trash"></i>
<i class="fas fa-eye"></i>
```

### Detail Pages / Modals
```html
<!-- Modal header -->
<i class="fas fa-file-invoice me-2"></i>

<!-- Section headers -->
<h6><i class="fas fa-list me-2"></i>Line Items</h6>
<h6><i class="fas fa-credit-card me-2"></i>Payments</h6>
<h6><i class="fas fa-sticky-note me-2"></i>Notes</h6>
```

### Navigation (Sidebar)
```html
<i class="fas fa-tachometer-alt me-2"></i> Dashboard
<i class="fas fa-users me-2"></i> Patients
<i class="fas fa-user-md me-2"></i> Doctors
<i class="fas fa-calendar-alt me-2"></i> Appointments
<i class="fas fa-file-medical me-2"></i> Medical Records
<i class="fas fa-dollar-sign me-2"></i> Billing
<i class="fas fa-users-cog me-2"></i> Staff
```

## 📚 Icon Library

### Common Icons Used
```
👤 People
fa-user          - Patient, person
fa-users         - Multiple patients, groups
fa-user-md       - Doctor, physician
fa-user-plus     - Add patient
fa-users-cog     - Staff management

📅 Calendar
fa-calendar-alt    - Calendar, appointments
fa-calendar-plus   - Add appointment
fa-calendar-times  - No appointments (empty)
fa-clock          - Time, recent

💰 Financial
fa-dollar-sign         - Billing, money
fa-file-invoice-dollar - Invoice
fa-credit-card         - Payments

🏥 Medical
fa-file-medical      - Medical records
fa-file-medical-alt  - Medical file
fa-heartbeat         - Vital signs, health
fa-notes-medical     - Medical notes
fa-pills             - Prescriptions
fa-diagnoses         - Diagnosis
fa-clipboard-list    - Treatment plan

📄 Documents
fa-file-alt      - Document, file
fa-sticky-note   - Notes
fa-list          - Lists

⚙️ Actions
fa-plus              - Add, create
fa-edit              - Edit
fa-trash             - Delete
fa-eye               - View
fa-check             - Confirm, success
fa-times             - Close, cancel
fa-ban               - Prohibited, cancel
fa-search            - Search
fa-lock              - Locked
fa-unlock            - Unlocked

📊 Dashboard
fa-tachometer-alt  - Dashboard
fa-chart-line      - Analytics, overview
fa-bolt            - Quick actions

ℹ️ Information
fa-info-circle         - Information
fa-exclamation-circle  - Warning, alert
fa-exclamation-triangle - Warning
fa-check-circle        - Success
fa-shield-alt          - Security
```

## ✅ Checklist for New Icons

Before adding an icon to your code:

- [ ] Does it use NO color class? (or only `text-muted` for empty states)
- [ ] Does it use `me-2` for inline spacing?
- [ ] Does it use appropriate size (`fa-2x` or `fa-3x`) for context?
- [ ] Does it follow the established pattern for its context?
- [ ] Is it consistent with similar elements in the app?

## 🎯 Decision Tree

**Adding an inline icon?**
→ Use `me-2`, no size class, no color class

**Adding a dashboard statistic?**
→ Use `fa-3x mb-3`, no color class

**Adding a quick action button?**
→ Use `fa-2x mb-2`, no color class

**Adding an empty state?**
→ Use `fa-3x mb-3 text-muted`, muted is OK

**Adding a section header?**
→ Use `me-2` inside `<h6>`, no color class

**Adding an action button?**
→ No size, no spacing, no color (just the icon)

## 🚨 Common Mistakes

### Mistake #1: Adding Color Classes
```html
<!-- WRONG -->
<i class="fas fa-user text-primary"></i>

<!-- RIGHT -->
<i class="fas fa-user"></i>
```

### Mistake #2: Inconsistent Spacing
```html
<!-- WRONG -->
<i class="fas fa-user me-3"></i>

<!-- RIGHT -->
<i class="fas fa-user me-2"></i>
```

### Mistake #3: Wrong Size for Context
```html
<!-- WRONG: Dashboard card with default size -->
<i class="fas fa-users"></i>

<!-- RIGHT: Dashboard card with fa-3x -->
<i class="fas fa-users fa-3x mb-3"></i>
```

### Mistake #4: Color on Section Headers
```html
<!-- WRONG -->
<h6 class="text-primary"><i class="fas fa-list me-2"></i>Items</h6>

<!-- RIGHT -->
<h6><i class="fas fa-list me-2"></i>Items</h6>
```

## 📖 Reference

For complete details, see:
- `ICON_CONSISTENCY_AUDIT.md` - Full audit and analysis
- `ICON_IMPLEMENTATION_SUMMARY.md` - Implementation report
- `PROFESSIONAL_DESIGN_SYSTEM.md` - Overall design system
- `VISUAL_STYLE_GUIDE.md` - Visual style guidelines

---

**Remember**: Icons support content, they don't compete with it. Keep them monochromatic, consistent, and professional.
