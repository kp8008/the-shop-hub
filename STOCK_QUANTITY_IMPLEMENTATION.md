# Stock Quantity Feature Implementation

## Overview
Added Stock Quantity management feature to allow admins to add, edit, and track product inventory.

## Changes Made

### Backend Changes

#### 1. Product Model (ECommerceAPI/Models/Product.cs)
- Added `StockQuantity` property to Product entity:
```csharp
[Required]
public int StockQuantity { get; set; }
```

#### 2. ProductDTO (ECommerceAPI/Models/Product.cs)
- Added `StockQuantity` property to ProductDTO:
```csharp
public int StockQuantity { get; set; }
```

#### 3. ProductValidator (ECommerceAPI/Validators/ProductValidator.cs)
- Added validation rule for StockQuantity:
```csharp
RuleFor(x => x.StockQuantity)
    .GreaterThanOrEqualTo(0).WithMessage("StockQuantity must be >= 0.");
```

#### 4. ProductController (ECommerceAPI/Controllers/ProductController.cs)
- Updated INSERT endpoint to save StockQuantity
- Updated UPDATE endpoint to update StockQuantity
- Updated all GET endpoints to return StockQuantity in response:
  - GetAllProducts
  - GetAllProductsForAdmin
  - GetProductsByCategory
  - GetByIdProducts
  - SearchProducts

### Frontend Changes

#### 1. AdminProducts.jsx (the-shop-hub/src/pages/admin/AdminProducts.jsx)
- Added `stockQuantity` to form state
- Added Stock Quantity input field in the product form
- Updated `handleSubmit` to send StockQuantity to backend
- Updated `openModal` to load StockQuantity when editing
- Stock Quantity field features:
  - Required field
  - Minimum value: 0
  - Number input type
  - Default value: 0

### Database Migration

**IMPORTANT**: You need to add the StockQuantity column to your database!

#### Option 1: Run SQL Script (Recommended if backend is running)
1. Open SQL Server Management Studio or Azure Data Studio
2. Connect to your database
3. Open the file `ADD_STOCK_QUANTITY.sql`
4. Update the database name if needed (line 4)
5. Execute the script

#### Option 2: Use EF Core Migration (If backend is stopped)
1. Stop the backend application
2. Run: `dotnet ef migrations add AddStockQuantityToProduct`
3. Run: `dotnet ef database update`
4. Start the backend application

## Features

### Admin Can:
- ✅ Add products with stock quantity
- ✅ Edit product stock quantity
- ✅ View stock quantity in products list
- ✅ Stock quantity validation (must be >= 0)
- ✅ Default stock quantity is 0 for new products

### Display:
- Stock quantity shown in products table with color coding:
  - Green: > 10 units (In Stock)
  - Yellow: 1-10 units (Low Stock)
  - Red: 0 units (Out of Stock)

## Testing Instructions

### 1. Add Database Column
First, run the SQL script to add the StockQuantity column to your database.

### 2. Restart Backend
Stop and restart the ECommerceAPI to load the model changes.

### 3. Test Add Product
1. Login as admin
2. Navigate to Products page
3. Click "Add Product"
4. Fill in all fields including Stock Quantity
5. Submit the form
6. Verify product is created with correct stock quantity

### 4. Test Edit Product
1. Click edit icon on any product
2. Change the stock quantity
3. Submit the form
4. Verify stock quantity is updated

### 5. Test Validation
1. Try to add a product with negative stock quantity
2. Verify validation error is shown

## API Changes

### Request Format (Add/Edit Product)
```
POST/PUT /api/Product
Content-Type: multipart/form-data

ProductName: "Product Name"
ProductCode: "PROD-001"
Price: 99.99
StockQuantity: 100
CategoryID: 1
IsActive: true
DocumentFile: [file] (optional)
```

### Response Format (Get Products)
```json
{
  "productID": 1,
  "productName": "Product Name",
  "productCode": "PROD-001",
  "price": 99.99,
  "stockQuantity": 100,
  "image": "/Files/Products/image.jpg",
  "categoryID": 1,
  "categoryName": "Category Name",
  "userID": 1,
  "createdBy": "Admin",
  "isActive": true,
  "created": "2024-01-01T00:00:00",
  "modified": "2024-01-01T00:00:00"
}
```

## Files Modified

### Backend
- `ECommerceAPI/Models/Product.cs` - Added StockQuantity to Product and ProductDTO
- `ECommerceAPI/Validators/ProductValidator.cs` - Added StockQuantity validation
- `ECommerceAPI/Controllers/ProductController.cs` - Updated all endpoints to handle StockQuantity

### Frontend
- `the-shop-hub/src/pages/admin/AdminProducts.jsx` - Added Stock Quantity input field and handling

### Database
- `ADD_STOCK_QUANTITY.sql` - SQL script to add StockQuantity column

## Future Enhancements
- Automatic stock reduction when orders are placed
- Low stock alerts for admins
- Stock history tracking
- Bulk stock update feature
- Stock reservation for pending orders