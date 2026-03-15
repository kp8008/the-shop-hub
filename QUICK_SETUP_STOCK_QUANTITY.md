# Quick Setup: Stock Quantity Feature

## Step 1: Add Database Column

**Run this SQL in your database:**

```sql
USE [ECommerceDB]  -- Replace with your database name
GO

ALTER TABLE [dbo].[Products]
ADD [StockQuantity] INT NOT NULL DEFAULT 0
GO
```

**How to run:**
1. Open SQL Server Management Studio or Azure Data Studio
2. Connect to your database
3. Click "New Query"
4. Paste the SQL above (change database name if needed)
5. Click "Execute" or press F5

## Step 2: Restart Backend

1. Stop your ECommerceAPI (press Ctrl+C in terminal)
2. Start it again: `dotnet run` or press F5 in Visual Studio

## Step 3: Test It!

1. Login as admin
2. Go to Products page
3. Click "Add Product"
4. You'll now see a "Stock Quantity" field!
5. Add a product with stock quantity (e.g., 100)
6. Save and verify it shows in the products list

## That's It!

Your admin can now:
- ✅ Add products with stock quantity
- ✅ Edit stock quantity
- ✅ See stock levels (color-coded: green/yellow/red)

## Troubleshooting

**Error: "Invalid column name 'StockQuantity'"**
- You forgot to run the SQL script. Go back to Step 1.

**Stock Quantity field not showing in form**
- Clear your browser cache and refresh the page
- Make sure you're running the latest frontend code

**Backend won't start after changes**
- Check if you have any syntax errors
- Make sure the database column was added successfully