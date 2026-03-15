// Format price in Indian Rupees (₹)
export const formatPrice = (amount) => {
  if (amount == null || isNaN(amount)) return '₹0'
  return `₹${Number(amount).toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`
}
