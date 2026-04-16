# Layout & CSS Implementation Guide

## Dark Theme Applied ✓
- Primary Background: `#1a1a1a`
- Secondary Background: `#2a2a2a`
- Accent Color: `#ff6b35` (Orange)
- Text Color: `#ffffff`

---

## Layout Structure

### Master Layout (_Layout.cshtml)
- ✅ Sticky Navigation Bar with All Entities
- ✅ Breadcrumbs Navigation Component
- ✅ Main Content Area with @RenderBody()
- ✅ Footer with Company Info & Quick Links
- ✅ No Bootstrap - Pure Custom CSS

### Navigation Links
```
🏠 Home
🏪 Restaurants  
🍕 Menu Items
📦 Orders
👥 Customers
```

---

## CSS Features

### Grid Layouts
Use `.grid` with column variants:
- `.grid-cols-1` → 1 column (full width)
- `.grid-cols-2` → Responsive 2 columns
- `.grid-cols-3` → Responsive 3 columns (Desktop default)
- `.grid-cols-4` → 4 columns

**Mobile-first responsive:** All grids automatically collapse to 1 column on tablets/mobile.

### Card Component
```html
<div class="card">
  <img src="..." alt="..." class="card-image">
  <div class="card-header">
    <h3 class="card-title">Item Title</h3>
    <p class="card-subtitle">Subtitle</p>
  </div>
  <div class="card-body">
    <p class="card-description">Description text here...</p>
    <div class="card-meta">
      <div class="card-meta-item">
        <span class="card-meta-label">Price:</span>
        <span class="card-meta-value">$12.99</span>
      </div>
    </div>
  </div>
  <div class="card-footer">
    <a href="#" class="btn btn-primary">View Details</a>
    <a href="#" class="btn btn-secondary">More Info</a>
  </div>
</div>
```

### Button Styles
- `.btn-primary` → Orange (#ff6b35), black text, shadow on hover
- `.btn-secondary` → Dark with border, accent text on hover
- `.btn-outline` → Transparent with orange border
- `.btn-small` → Compact buttons
- `.btn-large` → Full-size action buttons

### Breadcrumb Navigation
```html
<!-- In view, set ViewData in Controller: -->
<!-- ViewData["Breadcrumbs"] = new List<(string, string)> 
{
  ("Home", "/Home"),
  ("Restaurants", "/Restaurant"),
  ("Details", "")
}; -->
```

Breadcrumbs automatically render in layout with "/" separators.

---

## Example: Restaurant Card Grid View

```html
@{
    ViewData["Title"] = "Restaurants";
}

<h1>Popular Restaurants</h1>

<div class="grid grid-cols-3">
    @foreach (var restaurant in Model)
    {
        <div class="card">
            <img src="@restaurant.ImageUrl" alt="@restaurant.Name" class="card-image">
            <div class="card-header">
                <h3 class="card-title">@restaurant.Name</h3>
                <p class="card-subtitle">⭐ @restaurant.Rating</p>
            </div>
            <div class="card-body">
                <p class="card-description">@restaurant.Description</p>
                <div class="card-meta">
                    <div class="card-meta-item">
                        <span class="card-meta-label">Cuisine:</span>
                        <span class="card-meta-value">@restaurant.CuisineType</span>
                    </div>
                </div>
            </div>
            <div class="card-footer">
                <a asp-controller="Restaurant" asp-action="Details" asp-route-id="@restaurant.Id" class="btn btn-primary">View Menu</a>
            </div>
        </div>
    }
</div>
```

---

## Typography Scale

| Element | Size | Weight |
|---------|------|--------|
| h1 | 2.5rem | 700 |
| h2 | 2rem | 700 |
| h3 | 1.5rem | 600 |
| h4 | 1.25rem | 600 |
| Body | 1rem | 400 |
| Small | 0.9rem | 400 |

---

## Color Palette

| Name | Hex | Usage |
|------|-----|-------|
| Primary | #1a1a1a | Background |
| Secondary | #2a2a2a | Cards, Header |
| Accent | #ff6b35 | Buttons, Links, Highlights |
| Text | #ffffff | Main text |
| Text Light | #b0b0b0 | Secondary text |
| Border | #3a3a3a | Dividers |

---

## Spacing Scale

- `xs`: 0.5rem (8px)
- `sm`: 1rem (16px)
- `md`: 1.5rem (24px)
- `lg`: 2rem (32px)
- `xl`: 3rem (48px)

Use utility classes: `.mt-md`, `.mb-lg`, `.pt-sm`, `.pb-md`, etc.

---

## Utility Classes

### Text Alignment
- `.text-center` → Center text
- `.text-left` → Left align
- `.text-right` → Right align

### Flexbox
- `.flex-center` → Center items (flex)
- `.flex-between` → Space-between items

### Colors
- `.color-accent` → Orange text
- `.color-text-light` → Light gray text
- `.bg-accent` → Orange background
- `.bg-secondary` → Dark background

### Responsive Behavior
All grid layouts and buttons automatically adjust on mobile devices (<768px).

---

## Features Summary

✅ Dark Modern Theme (#1a1a1a)
✅ Orange Accent Color (#ff6b35)
✅ Sticky Navigation with All Entities
✅ Breadcrumb Navigation Support
✅ Card-Based Layout System
✅ Responsive Grid (3→1 columns)
✅ Custom Button Styles
✅ Form Styling
✅ Footer Component
✅ Mobile-First Design
✅ No Bootstrap Dependencies
✅ Modern Typography (16px base)
✅ Smooth Transitions & Hover Effects
✅ Focus States for Accessibility
