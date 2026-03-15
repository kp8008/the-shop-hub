# Debug Stock Quantity Issue - Step by Step

## Step 1: Check Database Column

Run this SQL query in SQL Server Management Studio:

```sql
USE [ECommerceDB]  -- Replace with your database name
GO

-- Check if column exists
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Products' AND COLUMN_NAME = 'StockQuantity'
GO

-- Check actual data
SELECT TOP 5 ProductID, ProductName, StockQuantity
FROM Products
GO
```

**Expected Result:**
- Column should exist with DATA_TYPE = 'int'
- Products should have StockQuantity values (not all 0)

**If column doesn't exist:**
Run the `ADD_STOCK_QUANTITY.sql` script.

**If all values are 0:**
Run this to set test data:
```sql
UPDATE Products SET StockQuantity = 100
```

## Step 2: Check Backend Response

1. Open your browser (Chrome/Edge)
2. Press F12 to open Developer Tools
3. Go to "Network" tab
4. Refresh the products page
5. Look for the request to `/Product/admin`
6. Click on it and check the "Response" tab

**What to look for:**
```json
[
  {
    "productID": 1,
    "productName": "iPhone",
    "stockQuantity": 100,  // ← Should be here (lowercase)
    // OR
    "StockQuantity": 100,  // ← Or here (PascalCase)
    ...
  }
]
```

**If StockQuantity is missing from response:**
- Backend changes didn't take effect
- Restart backend: Stop (Ctrl+C) and run `dotnet run` again

## Step 3: Check Browser Console

1. Press F12 to open Developer Tools
2. Go to "Console" tab
3. Refresh the products page
4. Look for the debug logs:

```
=== PRODUCTS API RESPONSE ===
First product: {productID: 1, productName: "iPhone", ...}
StockQuantity field: 100
stockQuantity field: undefined
All fields: ["productID", "productName", "StockQuantity", ...]
============================
```

**What this tells you:**
- If `StockQuantity field: 100` → Backend is sending data correctly
- If `StockQuantity field: undefined` → Backend not sending data
- Check "All fields" to see exact field names

## Step 4: Verify Backend Code Changes

Check if these files have the changes:

### File: ECommerceAPI/Controllers/ProductController.cs

**Search for "INSERT PRODUCT" section:**
```csharp
var response = new
{
    addproduct.ProductID,
    addproduct.ProductName,
    addproduct.ProductCode,
    addproduct.Price,
    addproduct.StockQuantity,  // ← This line should be here
    addproduct.Image,
    // ...
};
```

**Search for "UPDATE PRODUCT" section:**
```csharp
var response = new
{
    updateproduct.ProductID,
    updateproduct.ProductName,
    updateproduct.ProductCode,
    updateproduct.Price,
    updateproduct.StockQuantity,  // ← This line should be here
    updateproduct.Image,
    // ...
};
```

**Search for "GET ALL PRODUCTS FOR ADMIN" section:**
```csharp
.Select(p => new
{
    p.ProductID,
    p.ProductName,
    p.ProductCode,
    p.Price,
    p.StockQuantity,  // ← This line should be here
    p.Image,
    // ...
})
```

**If any of these are missing:**
The backend changes weren't saved. Re-apply the changes.

## Step 5: Test Update Flow

1. Open browser console (F12 → Console tab)
2. Edit a product and change stock quantity to 50
3. Click Save
4. Watch the console for:
   - Request payload (should include StockQuantity: 50)
   - Response (should include stockQuantity: 50 or StockQuantity: 50)

## Step 6: Manual Database Update Test

If everything else looks good but still shows 0, manually update database:

```sql
-- Update a specific product
UPDATE Products 
SET StockQuantity = 150 
WHERE ProductID = 1
GO

-- Verify
SELECT ProductID, ProductName, StockQuantity 
FROM Products 
WHERE ProductID = 1
GO
```

Then refresh the products page. If it now shows 150, the issue is with the UPDATE endpoint.

## Common Issues and Solutions

### Issue 1: Column doesn't exist
**Solution:** Run `ADD_STOCK_QUANTITY.sql`

### Issue 2: Backend not restarted
**Solution:** 
1. Stop backend (Ctrl+C)
2. Run `dotnet run` in ECommerceAPI folder
3. Wait for "Now listening on: https://localhost:7077"

### Issue 3: Frontend cache
**Solution:**
1. Hard refresh: Ctrl+Shift+R (Windows) or Cmd+Shift+R (Mac)
2. Or clear browser cache
3. Or open in Incognito/Private window

### Issue 4: Wrong database
**Solution:** Check connection string in `appsettings.json` points to correct database

### Issue 5: Migration not applied
**Solution:** 
```bash
cd ECommerceAPI
dotnet ef database update
```

## Quick Test Commands

### Test 1: Check if backend is running
Open browser: `https://localhost:7077/api/Product/admin`
Should see JSON with products.

### Test 2: Check database directly
```sql
SELECT TOP 1 * FROM Products
```
Should see StockQuantity column.

### Test 3: Check frontend
Open browser console and type:
```javascript
fetch('https://localhost:7077/api/Product/admin', {
  headers: {
    'Authorization': 'Bearer YOUR_TOKEN_HERE'
  }
})
.then(r => r.json())
.then(d => console.log('First product:', d[0]))
```

## Still Not Working?

If you've tried everything above and it still shows 0:

1. **Take a screenshot of:**
   - Browser Network tab showing the API response
   - Browser Console tab showing the debug logs
   - SQL query result showing StockQuantity values

2. **Check these files match the latest code:**
   - `ECommerceAPI/Controllers/ProductController.cs`
   - `ECommerceAPI/Models/Product.cs`
   - `the-shop-hub/src/pages/admin/AdminProducts.jsx`

3. **Try this nuclear option:**
   - Stop backend
   - Stop frontend
   - Clear browser cache completely
   - Restart backend
   - Restart frontend
   - Hard refresh browser (Ctrl+Shift+R)