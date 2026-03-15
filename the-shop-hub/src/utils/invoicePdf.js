import jsPDF from 'jspdf';

// Format currency - use Rs. for better font compatibility
const formatCurrency = (amount) => {
  const num = parseFloat(amount) || 0;
  return `Rs. ${num.toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`;
};

// Format date
const formatDate = (dateString) => {
  if (!dateString) return 'N/A';
  const date = new Date(dateString);
  return date.toLocaleDateString('en-IN', {
    day: '2-digit',
    month: 'short',
    year: 'numeric'
  });
};

// Get user from localStorage
const getStoredUser = () => {
  try {
    const authData = localStorage.getItem('auth-storage');
    if (authData) {
      const parsed = JSON.parse(authData);
      return parsed?.state?.user || null;
    }
  } catch (e) {
    console.error('Error getting user:', e);
  }
  return null;
};

// Get status color
const getStatusColor = (status) => {
  const statusLower = (status || 'pending').toLowerCase();
  switch (statusLower) {
    case 'delivered':
      return { r: 34, g: 197, b: 94 }; // Green
    case 'shipped':
      return { r: 59, g: 130, b: 246 }; // Blue
    case 'processing':
      return { r: 249, g: 115, b: 22 }; // Orange
    case 'cancelled':
      return { r: 239, g: 68, b: 68 }; // Red
    default:
      return { r: 107, g: 114, b: 128 }; // Gray
  }
};

export const downloadInvoicePdf = (order, items, customerType = 'customer') => {
  const doc = new jsPDF({
    orientation: 'portrait',
    unit: 'mm',
    format: 'a4'
  });

  const pageWidth = doc.internal.pageSize.getWidth();
  const margin = 20;
  const contentWidth = pageWidth - (margin * 2);
  
  // Get user info - from order for admin, from localStorage for customer
  const storedUser = getStoredUser();
  const isAdmin = customerType === 'admin';
  
  // For admin: use customer details from order; For customer: use logged in user
  const user = isAdmin ? {
    name: order.userName || order.UserName || 'Customer',
    email: order.userEmail || order.UserEmail || '',
    phoneNumber: order.userPhone || order.UserPhone || order.phone || ''
  } : storedUser;
  
  // Get items from order or passed items
  const orderItems = items || order.items || order.orderDetails || [];
  
  // Get address
  const address = order.shippingAddress || order.address || {};
  
  // Status
  const status = order.status || order.orderStatus || 'Pending';
  const statusColor = getStatusColor(status);

  let y = margin;

  // ===== HEADER =====
  // Company name
  doc.setFont('helvetica', 'bold');
  doc.setFontSize(24);
  doc.setTextColor(79, 70, 229); // Primary purple
  doc.text('The Shop Hub', margin, y);
  
  y += 8;
  doc.setFont('helvetica', 'normal');
  doc.setFontSize(10);
  doc.setTextColor(107, 114, 128);
  doc.text('Your Premium Shopping Destination', margin, y);

  // Invoice title on right
  doc.setFont('helvetica', 'bold');
  doc.setFontSize(28);
  doc.setTextColor(17, 24, 39);
  doc.text('INVOICE', pageWidth - margin, margin, { align: 'right' });

  // Order number below invoice
  doc.setFontSize(11);
  doc.setTextColor(107, 114, 128);
  doc.text(`#${order.orderNo || order.orderId}`, pageWidth - margin, margin + 8, { align: 'right' });

  // Status badge
  y = margin + 18;
  const statusText = status.toUpperCase();
  doc.setFontSize(9);
  const statusWidth = doc.getTextWidth(statusText) + 10;
  doc.setFillColor(statusColor.r, statusColor.g, statusColor.b);
  doc.roundedRect(pageWidth - margin - statusWidth, y - 5, statusWidth, 8, 2, 2, 'F');
  doc.setTextColor(255, 255, 255);
  doc.setFont('helvetica', 'bold');
  doc.text(statusText, pageWidth - margin - statusWidth + 5, y);

  // Horizontal line
  y += 15;
  doc.setDrawColor(229, 231, 235);
  doc.setLineWidth(0.5);
  doc.line(margin, y, pageWidth - margin, y);

  // ===== ORDER & CUSTOMER INFO - TWO COLUMNS =====
  y += 12;
  const col1X = margin;
  const col2X = pageWidth / 2 + 10;

  // Left column - Order Details
  doc.setFont('helvetica', 'bold');
  doc.setFontSize(12);
  doc.setTextColor(17, 24, 39);
  doc.text('Order Details', col1X, y);

  y += 8;
  doc.setFont('helvetica', 'normal');
  doc.setFontSize(10);
  
  // Order Date
  doc.setTextColor(107, 114, 128);
  doc.text('Order Date:', col1X, y);
  doc.setTextColor(55, 65, 81);
  doc.text(formatDate(order.orderDate), col1X + 30, y);
  
  y += 6;
  // Delivery Date
  doc.setTextColor(107, 114, 128);
  doc.text('Delivery:', col1X, y);
  doc.setTextColor(55, 65, 81);
  doc.text(order.deliveryDate ? formatDate(order.deliveryDate) : 'Pending', col1X + 30, y);
  
  y += 6;
  // Payment Mode
  const payment = order.payment || order.Payment;
  const paymentModeName = payment?.paymentModeName ?? payment?.PaymentModeName ?? 'Cash on Delivery';
  doc.setTextColor(107, 114, 128);
  doc.text('Payment:', col1X, y);
  doc.setTextColor(55, 65, 81);
  doc.text(paymentModeName, col1X + 30, y);

  y += 6;
  // Payment Status & Reference (if available)
  const paymentStatus = payment?.paymentStatus ?? payment?.PaymentStatus ?? '';
  const paymentRef = payment?.paymentReference ?? payment?.PaymentReference ?? '';
  const transactionId = payment?.transactionID ?? payment?.TransactionID ?? '';
  if (paymentStatus || paymentRef || transactionId) {
    if (paymentStatus) {
      doc.setTextColor(107, 114, 128);
      doc.text('Status:', col1X, y);
      doc.setTextColor(55, 65, 81);
      doc.text(paymentStatus, col1X + 30, y);
      y += 5;
    }
    if (paymentRef) {
      doc.setTextColor(107, 114, 128);
      doc.text('Reference:', col1X, y);
      doc.setTextColor(55, 65, 81);
      const refStr = paymentRef.length > 28 ? paymentRef.substring(0, 25) + '...' : paymentRef;
      doc.text(refStr, col1X + 30, y);
      y += 5;
    }
    if (transactionId) {
      doc.setTextColor(107, 114, 128);
      doc.text('Txn ID:', col1X, y);
      doc.setTextColor(55, 65, 81);
      const txnStr = transactionId.length > 28 ? transactionId.substring(0, 25) + '...' : transactionId;
      doc.text(txnStr, col1X + 30, y);
      y += 6;
    }
  }

  // Right column - Customer Details
  let rightY = y - 20;
  doc.setFont('helvetica', 'bold');
  doc.setFontSize(12);
  doc.setTextColor(17, 24, 39);
  doc.text('Customer Details', col2X, rightY);

  rightY += 10;
  doc.setFont('helvetica', 'normal');
  doc.setFontSize(10);
  
  // Customer Name
  doc.setTextColor(107, 114, 128);
  doc.text('Name:', col2X, rightY);
  doc.setTextColor(55, 65, 81);
  const customerName = user?.name || user?.email?.split('@')[0] || 'Customer';
  doc.text(customerName, col2X + 20, rightY);
  
  rightY += 6;
  // Email
  doc.setTextColor(107, 114, 128);
  doc.text('Email:', col2X, rightY);
  doc.setTextColor(55, 65, 81);
  doc.text(user?.email || 'N/A', col2X + 20, rightY);
  rightY += 6;
  
  // Phone
  doc.setTextColor(107, 114, 128);
  doc.text('Phone:', col2X, rightY);
  doc.setTextColor(55, 65, 81);
  doc.text(user?.phoneNumber || 'N/A', col2X + 20, rightY);
  rightY += 6;

  // Address
  doc.setTextColor(107, 114, 128);
  doc.text('Address:', col2X, rightY);
  rightY += 5;
  const fullAddress = address.FullAddress || address.fullAddress || 
    [address.street, address.city, address.state, address.pincode].filter(Boolean).join(', ') || 
    'N/A';
  
  // Wrap long address
  const addressLines = doc.splitTextToSize(fullAddress, 70);
  doc.setTextColor(55, 65, 81);
  addressLines.forEach((line, i) => {
    doc.text(line, col2X, rightY + (i * 5));
  });

  // ===== ORDER ITEMS TABLE =====
  y = Math.max(y, rightY + (addressLines.length * 5)) + 20;
  
  // Table header
  doc.setFillColor(249, 250, 251);
  doc.rect(margin, y - 5, contentWidth, 10, 'F');
  
  doc.setFont('helvetica', 'bold');
  doc.setFontSize(10);
  doc.setTextColor(55, 65, 81);
  doc.text('Product', margin + 5, y);
  doc.text('Qty', margin + 100, y);
  doc.text('Price', margin + 120, y);
  doc.text('Total', margin + 150, y);

  y += 10;
  
  // Table items
  let subtotal = 0;
  
  orderItems.forEach((item, index) => {
    const productName = item.productName || item.ProductName || `Product #${item.productId || item.ProductId}`;
    const quantity = item.quantity || item.Quantity || 1;
    // Try multiple price fields: amount, netAmount, price, productPrice
    const price = item.amount || item.Amount || item.netAmount || item.NetAmount || item.price || item.Price || item.productPrice || item.ProductPrice || 0;
    const itemTotal = item.netAmount || item.NetAmount || (price * quantity);
    subtotal += itemTotal;

    // Alternate row background
    if (index % 2 === 0) {
      doc.setFillColor(249, 250, 251);
      doc.rect(margin, y - 4, contentWidth, 10, 'F');
    }

    doc.setFont('helvetica', 'normal');
    doc.setFontSize(10);
    doc.setTextColor(17, 24, 39);
    
    // Truncate long product names
    const truncatedName = productName.length > 40 ? productName.substring(0, 40) + '...' : productName;
    doc.text(truncatedName, margin + 5, y);
    
    doc.setTextColor(107, 114, 128);
    doc.text(quantity.toString(), margin + 100, y);
    doc.text(formatCurrency(price), margin + 120, y);
    
    doc.setFont('helvetica', 'bold');
    doc.setTextColor(17, 24, 39);
    doc.text(formatCurrency(itemTotal), margin + 150, y);

    y += 10;
  });

  // If no items, show message
  if (orderItems.length === 0) {
    doc.setFont('helvetica', 'italic');
    doc.setFontSize(10);
    doc.setTextColor(156, 163, 175);
    doc.text('No items in order', margin + 5, y);
    y += 10;
    subtotal = order.totalAmount || 0;
  }

  // ===== TOTALS SECTION =====
  y += 10;
  doc.setDrawColor(229, 231, 235);
  doc.line(margin + 100, y, pageWidth - margin, y);
  
  y += 10;
  const totalsX = margin + 120;
  
  // Subtotal
  doc.setFont('helvetica', 'normal');
  doc.setFontSize(10);
  doc.setTextColor(107, 114, 128);
  doc.text('Subtotal:', totalsX, y);
  doc.setTextColor(55, 65, 81);
  doc.text(formatCurrency(subtotal), totalsX + 35, y);
  
  y += 7;
  // Tax (18% GST)
  const taxRate = 0.18;
  const taxAmount = subtotal * taxRate;
  doc.setTextColor(107, 114, 128);
  doc.text('Tax (18% GST):', totalsX, y);
  doc.setTextColor(55, 65, 81);
  doc.text(formatCurrency(taxAmount), totalsX + 35, y);
  
  y += 7;
  // Shipping
  doc.setTextColor(107, 114, 128);
  doc.text('Shipping:', totalsX, y);
  doc.setTextColor(34, 197, 94);
  doc.text('FREE', totalsX + 35, y);
  
  y += 7;
  // Discount (if any)
  const discount = 0;
  if (discount > 0) {
    doc.setTextColor(107, 114, 128);
    doc.text('Discount:', totalsX, y);
    doc.setTextColor(239, 68, 68);
    doc.text(`-${formatCurrency(discount)}`, totalsX + 35, y);
    y += 7;
  }

  // Total line
  y += 3;
  doc.setDrawColor(79, 70, 229);
  doc.setLineWidth(0.5);
  doc.line(totalsX - 5, y, pageWidth - margin, y);
  
  y += 10;
  // Grand Total - use order total which includes tax
  doc.setFont('helvetica', 'bold');
  doc.setFontSize(14);
  doc.setTextColor(17, 24, 39);
  doc.text('Grand Total:', totalsX, y);
  doc.setTextColor(79, 70, 229);
  const grandTotal = order.totalAmount || (subtotal + taxAmount);
  doc.text(formatCurrency(grandTotal), totalsX + 35, y);

  // ===== FOOTER =====
  const footerY = doc.internal.pageSize.getHeight() - 25;
  
  // Thank you message
  doc.setFont('helvetica', 'bold');
  doc.setFontSize(12);
  doc.setTextColor(79, 70, 229);
  doc.text('Thank you for your purchase!', pageWidth / 2, footerY, { align: 'center' });
  
  doc.setFont('helvetica', 'normal');
  doc.setFontSize(9);
  doc.setTextColor(156, 163, 175);
  doc.text('For any queries, please contact support@theshophub.com', pageWidth / 2, footerY + 6, { align: 'center' });
  doc.text('www.theshophub.com', pageWidth / 2, footerY + 11, { align: 'center' });

  // Save the PDF
  doc.save(`invoice-${order.orderNo || order.orderId}.pdf`);
};

// Export both named and default
export const generateInvoice = downloadInvoicePdf;
export default downloadInvoicePdf;
