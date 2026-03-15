# The Shop Hub 🛍️

A modern, beautiful e-commerce platform built with React, Vite, and Tailwind CSS. Features stunning animations, glassmorphism effects, and a complete admin panel.

## ✨ Features

### User Side
- 🏠 Beautiful homepage with hero section and featured products
- 🛒 Product browsing with advanced filters and sorting
- 🔍 Product detail pages with related products
- 🛍️ Shopping cart with quantity management
- 💳 Multi-step checkout process
- 👤 User profile management
- 📦 Order history tracking
- 🔐 Secure authentication (Login/Register)

### Admin Side
- 📊 Dashboard with statistics and analytics
- 📦 Product management (CRUD operations)
- 🏷️ Category management
- 📋 Order management with status updates
- 👥 User management
- 🖼️ Image upload support

### Design Features
- 🎨 Modern UI inspired by Shopify, Amazon, and Flipkart
- ✨ Smooth animations with Framer Motion
- 🌈 Glassmorphism effects
- 📱 Fully responsive design
- 🎭 Micro-interactions and hover effects
- 🌊 Floating elements and gradient backgrounds
- 🎯 Clean and intuitive navigation

## 🚀 Tech Stack

- **React 18** - UI library
- **Vite** - Build tool
- **Tailwind CSS** - Styling
- **Framer Motion** - Animations
- **React Router DOM** - Routing
- **Zustand** - State management
- **Axios** - HTTP client
- **React Hot Toast** - Notifications
- **Lucide React** - Icons

## 📋 Prerequisites

- Node.js (v16 or higher)
- npm or yarn
- .NET backend API running on `http://localhost:5000`

## 🛠️ Installation

1. **Navigate to the project directory:**
   ```bash
   cd the-shop-hub
   ```

2. **Install dependencies:**
   ```bash
   npm install
   ```

3. **Configure API endpoint:**
   - Open `src/utils/api.js`
   - Update `API_BASE_URL` if your backend runs on a different port

4. **Start the development server:**
   ```bash
   npm run dev
   ```

5. **Open your browser:**
   - Navigate to `http://localhost:3000`

## 🏗️ Build for Production

```bash
npm run build
```

The build files will be in the `dist` directory.

## 📁 Project Structure

```
the-shop-hub/
├── src/
│   ├── components/
│   │   ├── admin/          # Admin components
│   │   └── user/           # User components
│   ├── layouts/
│   │   ├── AdminLayout.jsx
│   │   └── UserLayout.jsx
│   ├── pages/
│   │   ├── admin/          # Admin pages
│   │   ├── auth/           # Authentication pages
│   │   └── user/           # User pages
│   ├── store/              # Zustand stores
│   ├── utils/              # Utilities and API config
│   ├── App.jsx
│   ├── main.jsx
│   └── index.css
├── public/
├── index.html
├── package.json
├── tailwind.config.js
└── vite.config.js
```

## 🎯 Key Features Explained

### State Management
- **Zustand** for lightweight state management
- Persistent cart and auth state using localStorage
- Automatic token injection in API requests

### Animations
- Page transitions with Framer Motion
- Hover effects on cards and buttons
- Smooth scroll animations
- Loading skeletons

### Responsive Design
- Mobile-first approach
- Breakpoints: sm (640px), md (768px), lg (1024px), xl (1280px)
- Collapsible mobile menu
- Adaptive layouts

## 🔐 Authentication

### User Roles
- **Admin** (userTypeId: 1) - Full access to admin panel
- **Customer** (userTypeId: 2) - Shopping and order management

### Default Credentials
Create an admin user through your backend API or register and update the userTypeId in the database.

## 🎨 Customization

### Colors
Edit `tailwind.config.js` to customize the color scheme:
```js
colors: {
  primary: {
    // Your custom colors
  }
}
```

### Animations
Modify animation timings in `tailwind.config.js`:
```js
animation: {
  'fade-in': 'fadeIn 0.5s ease-in-out',
  // Add your custom animations
}
```

## 🐛 Troubleshooting

### API Connection Issues
- Ensure your .NET backend is running
- Check CORS settings in your backend
- Verify the API_BASE_URL in `src/utils/api.js`

### Build Errors
- Clear node_modules and reinstall: `rm -rf node_modules && npm install`
- Clear Vite cache: `rm -rf node_modules/.vite`

## 📝 API Endpoints Used

- `/Auth/login` - User login
- `/Auth/register` - User registration
- `/Product` - Product CRUD
- `/Category` - Category CRUD
- `/Order` - Order management
- `/User` - User management
- `/Cart` - Cart operations
- `/Payment` - Payment processing

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Open a pull request

## 📄 License

This project is licensed under the MIT License.

## 🙏 Acknowledgments

- Design inspiration from Shopify, Amazon, and Flipkart
- Icons by Lucide React
- Animations by Framer Motion

## 📞 Support

For issues and questions, please open an issue on GitHub.

---

Made with ❤️ by The Shop Hub Team
