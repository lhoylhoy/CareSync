# Actions Column Consistency Guide

## Overview
This document defines the standardized implementation for Actions columns across all CRUD pages in the CareSync application, ensuring professional monochromatic design consistency.

**Last Updated:** 2024
**Status:** ✅ Fully Implemented

---

## Design Principles

### 1. **Monochromatic Styling**
- All action buttons use `btn-outline-secondary` (slate-based monochromatic)
- No colored buttons (success, warning, primary, danger, info)
- Maintains professional, minimalist aesthetic

### 2. **Consistent Sizing**
- All button groups use `btn-group-sm` class
- All buttons within groups use `btn-sm` class
- Ensures compact, uniform appearance across all pages

### 3. **Icon-Only Actions**
- Buttons use FontAwesome icons without text labels
- Icons are semantic and intuitive
- Title attributes provide hover tooltips for clarity

---

## Standard Implementation

### Default Actions Template (CrudTableComponent.razor)

Used by: `Patients.razor`, `Doctors.razor`, `Staff.razor`, `Billing.razor`

```razor
<div class="btn-group btn-group-sm" role="group">
    @if (AllowEdit)
    {
        <button class="btn btn-outline-secondary btn-sm"
                @onclick="() => OnEditClicked.InvokeAsync(item)"
                title="Edit">
            <i class="fas fa-edit"></i>
        </button>
    }
    @if (AllowDelete)
    {
        <button class="btn btn-outline-secondary btn-sm"
                disabled="@disableDelete"
                @onclick="() => { if(!disableDelete) OnDeleteClicked.InvokeAsync(item); }"
                title="Delete">
            <i class="fas fa-trash"></i>
        </button>
    }
    @if (AllowView)
    {
        <button class="btn btn-outline-secondary btn-sm"
                @onclick="() => OnViewClicked.InvokeAsync(item)"
                title="View Details">
            <i class="fas fa-eye"></i>
        </button>
    }
</div>
```

**Features:**
- ✅ Edit, Delete, View actions
- ✅ Monochromatic slate buttons
- ✅ Small button sizing (btn-sm)
- ✅ Conditional delete disable for related data
- ✅ Semantic icons with tooltips

---

### Custom Actions Templates

#### Appointments.razor

```razor
<ActionsTemplate Context="appointment">
    <div class="btn-group btn-group-sm" role="group">
        @if (appointment.Status is AppointmentStatus.Scheduled or AppointmentStatus.InProgress)
        {
            <button class="btn btn-outline-secondary btn-sm" title="Mark as Completed"
                @onclick="() => CompleteAppointmentAsync(appointment)">
                <i class="fas fa-check"></i>
            </button>
        }
        @if (appointment.Status == AppointmentStatus.Scheduled)
        {
            <button class="btn btn-outline-secondary btn-sm" title="Cancel Appointment"
                @onclick="() => PromptCancelAsync(appointment)">
                <i class="fas fa-ban"></i>
            </button>
        }
    </div>
</ActionsTemplate>
```

**Features:**
- ✅ Context-specific actions (Complete, Cancel)
- ✅ Status-based conditional rendering
- ✅ Monochromatic styling
- ✅ Consistent sizing

#### MedicalRecords.razor

```razor
<ActionsTemplate Context="record">
    <div class="btn-group btn-group-sm" role="group">
        <button class="btn btn-outline-secondary btn-sm" title="View Details"
            @onclick="() => HandleRecordViewed(record)">
            <i class="fas fa-eye"></i>
        </button>
        @if (!record.IsFinalized)
        {
            <button class="btn btn-outline-secondary btn-sm" title="Finalize Record"
                @onclick="() => PromptFinalizeAsync(record)">
                <i class="fas fa-lock"></i>
            </button>
        }
        else
        {
            <button class="btn btn-outline-secondary btn-sm" title="Reopen Record"
                @onclick="() => PromptReopenAsync(record)">
                <i class="fas fa-unlock"></i>
            </button>
        }
    </div>
</ActionsTemplate>
```

**Features:**
- ✅ View, Finalize/Reopen actions
- ✅ State-based conditional rendering
- ✅ Monochromatic styling
- ✅ Semantic lock/unlock icons

---

## Icon Standards

### Standard Action Icons

| Action | Icon | Title |
|--------|------|-------|
| Edit | `fas fa-edit` | "Edit" |
| Delete | `fas fa-trash` | "Delete" |
| View | `fas fa-eye` | "View Details" |
| Complete | `fas fa-check` | "Mark as Completed" |
| Cancel | `fas fa-ban` | "Cancel" |
| Finalize | `fas fa-lock` | "Finalize Record" |
| Reopen | `fas fa-unlock` | "Reopen Record" |

### Icon Guidelines
- Use FontAwesome 6.x solid icons (`fas`)
- Choose semantic, universally understood icons
- No colored icon classes (text-primary, text-success, etc.)
- Consistent icon sizing via button sizing

---

## Button Styling Reference

### Class Combinations

```html
<!-- Standard Action Button -->
<button class="btn btn-outline-secondary btn-sm" title="Action Name">
    <i class="fas fa-icon-name"></i>
</button>

<!-- Button Group Wrapper -->
<div class="btn-group btn-group-sm" role="group">
    <!-- buttons here -->
</div>
```

### CSS Classes Used

| Class | Purpose |
|-------|---------|
| `btn` | Base Bootstrap button |
| `btn-outline-secondary` | Monochromatic slate outline style |
| `btn-sm` | Small button size |
| `btn-group` | Groups buttons together |
| `btn-group-sm` | Small button group size |

### Color Palette

```css
/* btn-outline-secondary uses slate monochromatic palette */
--cs-slate-600: #475569;  /* Primary slate */
--cs-slate-700: #334155;  /* Hover state */
--cs-slate-100: #f1f5f9;  /* Background on hover */
```

---

## Implementation Checklist

When creating new CRUD pages with actions:

### Standard Actions (Edit, Delete, View)
- [ ] Use default `ActionsTemplate` from `CrudTableComponent`
- [ ] Set `AllowEdit`, `AllowDelete`, `AllowView` parameters
- [ ] No custom action template needed

### Custom Actions
- [ ] Define `<ActionsTemplate Context="item">` in page
- [ ] Use `btn-group btn-group-sm` wrapper
- [ ] All buttons use `btn btn-outline-secondary btn-sm`
- [ ] Add semantic FontAwesome icons
- [ ] Provide descriptive `title` attributes
- [ ] Implement conditional rendering if needed
- [ ] Wire up `@onclick` event handlers

### Quality Assurance
- [ ] No colored button classes (success, warning, primary, danger, info)
- [ ] Consistent button sizing across all actions
- [ ] Icons are semantically appropriate
- [ ] Tooltips are clear and concise
- [ ] Hover states work correctly
- [ ] Conditional logic functions properly

---

## Files Modified

| File | Lines | Change Description |
|------|-------|-------------------|
| `CrudTableComponent.razor` | 56-85 | Updated default actions to `btn-outline-secondary`, added `btn-group-sm` |
| `Appointments.razor` | 83-100 | Replaced colored buttons with monochromatic slate |
| `MedicalRecords.razor` | 82-100 | Replaced colored buttons with monochromatic slate |

---

## Page Coverage

| Page | Actions Type | Status |
|------|-------------|--------|
| **Patients** | Default (Edit, Delete, View) | ✅ Monochromatic |
| **Doctors** | Default (Edit, Delete, View) | ✅ Monochromatic |
| **Staff** | Default (Edit, Delete, View) | ✅ Monochromatic |
| **Billing** | Default (Edit, Delete, View) | ✅ Monochromatic |
| **Appointments** | Custom (Complete, Cancel) | ✅ Monochromatic |
| **MedicalRecords** | Custom (View, Finalize, Reopen) | ✅ Monochromatic |

---

## Visual Consistency Compliance

### Before Actions Update
```html
<!-- ❌ INCONSISTENT - Mixed colors and sizing -->
<div class="btn-group" role="group">
    <button class="btn btn-outline-primary btn-sm">...</button>
    <button class="btn btn-outline-danger btn-sm">...</button>
    <button class="btn btn-outline-info btn-sm">...</button>
</div>

<div class="btn-group btn-group-sm" role="group">
    <button class="btn btn-outline-success">...</button>
    <button class="btn btn-outline-warning">...</button>
</div>
```

### After Actions Update
```html
<!-- ✅ CONSISTENT - Monochromatic slate, uniform sizing -->
<div class="btn-group btn-group-sm" role="group">
    <button class="btn btn-outline-secondary btn-sm">...</button>
    <button class="btn btn-outline-secondary btn-sm">...</button>
    <button class="btn btn-outline-secondary btn-sm">...</button>
</div>
```

---

## Design System Integration

This Actions consistency implementation aligns with:

- **PROFESSIONAL_DESIGN_SYSTEM.md** - Monochromatic color palette
- **ICON_USAGE_GUIDE.md** - Icon standardization and color neutrality
- **VISUAL_STYLE_GUIDE.md** - Component consistency principles
- **caresync.css** - Slate button variants and spacing

---

## Related Documentation

- [Professional Design System](./PROFESSIONAL_DESIGN_SYSTEM.md)
- [Icon Consistency Audit](./ICON_CONSISTENCY_AUDIT.md)
- [Icon Usage Guide](./ICON_USAGE_GUIDE.md)
- [Visual Style Guide](./VISUAL_STYLE_GUIDE.md)

---

## Maintenance Notes

### When Adding New Pages
1. Use `BaseCrudPage` component with default actions when possible
2. Only create custom `ActionsTemplate` if business logic requires unique actions
3. Always follow monochromatic button styling
4. Maintain consistent sizing (`btn-group-sm`, `btn-sm`)

### When Modifying Existing Actions
1. Never introduce colored button variants (success, warning, etc.)
2. Preserve semantic icon usage
3. Keep tooltips clear and actionable
4. Test conditional rendering logic

---

**Consistency Score:** 100% ✅
**Compliance:** Full monochromatic design system adherence
**Implementation:** Complete across all 6 CRUD pages
