# The Shop Hub – Live Deployment Guide (Resume / Portfolio)

This guide helps you host **database**, **backend API**, and **frontend** so you get **live URLs** to put on your resume (e.g. **Live:** `https://your-app.vercel.app` | **API:** `https://yourapi.onrender.com`).

Your faculty’s reference:  
**[Lab23 | ASP.NET Core Web API Live](https://github.com/sejalgupta001/AdvanceDotNet/blob/main/Lab23%20%7C%20ASP.NET%20Core%20Web%20API%20Live.md)**

---

## Overview (3 steps)

| Step | What | Where | Free? |
|------|------|--------|-------|
| 1 | Host **SQL Server database** | Somee.com | Yes (free tier) |
| 2 | Deploy **ECommerceAPI** (backend) | Render.com (Docker) | Yes (free tier, sleeps after 15 min) |
| 3 | Deploy **The Shop Hub** (React frontend) | Vercel or Netlify | Yes |

---

## Part 1 — Database live (Somee.com)

1. **Video (recommended)**  
   Watch: [Database Hosting on Somee](https://www.youtube.com/watch?v=XS2uTMzKhtI)

2. **Create account**  
   Go to [Somee.com](https://www.somee.com) → Register with email → Verify.

3. **Create database**  
   In Somee dashboard: **SQL Server** → **Create Database** → set name → save **Server name, Database name, Username, Password**.

4. **Import your data**  
   - In **SQL Server Management Studio (SSMS)** connect using Somee’s server & credentials.  
   - Restore your `.bak` **or** run your database script so tables match your API.

5. **Update API connection string**  
   In `ECommerceAPI/appsettings.json` (and for Render, use **Environment Variables** – see Part 2):

   ```json
   "ConnectionStrings": {
     "Default": "Server=SQLXXXX.somee.com;Database=YourDB;User Id=YourUser;Password=YourPassword;TrustServerCertificate=True;"
   }
   ```

---

## Part 2 — Backend API live (Render + Docker)

Follow your faculty doc for detailed steps; summary:

1. **Push code to GitHub**  
   - In Visual Studio: **Git → Create Git Repository** → Sign in to GitHub → Create repo → **Create and Push**.  
   - Or: create repo on GitHub, then push your `ADN` folder (with `ECommerceAPI`, `the-shop-hub`, and the **Dockerfile** at root).

2. **Dockerfile (already in project)**  
   Root folder `ADN` has a **Dockerfile** that builds and runs **ECommerceAPI**.  
   - Render will use this to build the API.  
   - Do **not** put secrets (DB password, JWT key) in the repo; use **Environment Variables** on Render.

3. **Deploy on Render**  
   - Go to [Render Dashboard](https://dashboard.render.com/) → **New +** → **Web Service**.  
   - Connect your **GitHub** account and select the repo that contains `ADN` (and the Dockerfile).  
   - Set:
     - **Name:** e.g. `the-shop-hub-api`
     - **Environment:** **Docker**
     - **Dockerfile path:** `Dockerfile` (root of repo)
     - **Instance type:** **Free**
   - Under **Environment**, add variables (replace with your real values):
     - `ConnectionStrings__Default` = your Somee connection string  
     - `JwtSettings__SecretKey` = your long secret key  
     - (If you use Cloudinary in production) `CloudinarySettings__CloudName`, `ApiKey`, `ApiSecret`
   - Click **Create Web Service**.  
   - After deploy, note the URL, e.g. `https://the-shop-hub-api.onrender.com`.

4. **Test API**  
   Open `https://your-api.onrender.com` → Swagger UI should open (we enabled it in production for demo).

---

## Part 3 — Frontend live (Vercel / Netlify)

1. **Build with production API URL**  
   In `the-shop-hub` create/update `.env.production` (or use build-time env in Vercel/Netlify):

   ```env
   VITE_API_BASE_URL=https://your-api.onrender.com/api
   VITE_GOOGLE_CLIENT_ID=your-google-client-id
   ```

   Replace `https://your-api.onrender.com` with your **actual Render API URL** (no trailing `/api` in the base; the app adds `/api`).

2. **Deploy on Vercel (recommended)**  
   - Go to [Vercel](https://vercel.com) → Sign up with GitHub.  
   - **Add New Project** → Import the **same repo**.  
   - **Root Directory:** set to `the-shop-hub` (so Vercel builds the React app only).  
   - **Framework Preset:** Vite.  
   - **Environment Variables:**  
     - `VITE_API_BASE_URL` = `https://your-api.onrender.com/api`  
     - `VITE_GOOGLE_CLIENT_ID` = your Google OAuth client ID  
   - Deploy. Your site will be like `https://the-shop-hub-xxx.vercel.app`.

3. **Or use Netlify**  
   - [Netlify](https://netlify.com) → Add new site from Git → select repo.  
   - Base directory: `the-shop-hub`.  
   - Build command: `npm run build`. Publish directory: `dist`.  
   - Add the same env vars in **Site settings → Environment variables**.

---

## Live hone pachi flow (તમે link મોકલો, friend use કરે, order તમને admin માં દેખાય)

જ્યારે database + API + frontend ત્રણે live થઈ જાય, ત્યારે:

1. **તમે મિત્રને ફક્ત એક link મોકલો** – frontend વાળો (જેમ કે `https://the-shop-hub-xxx.vercel.app`). API link મોકલવાની જરૂર નથી.

2. **મિત્ર શું કરશે (Customer):**
   - Site ખોલશે → **Sign Up** → **Customer** તરીકે register
   - Products browse કરશે → **Add to Cart** → Cart → **Checkout** → Address add → **Place Order**
   - બધું same app માં; order તમારા live database (Somee) માં save થશે.

3. **તમે શું કરશો (Admin):**
   - Same site પર જ **Login** → **Admin** select કરો (તમારો admin account પહેલેથી register કરેલો હોવો જોઈએ).
   - **Admin Dashboard** → **Orders** પર જાઓ → બધા orders (મિત્રનો + બીજા કોઈનો) દેખાશે, status update કરી શકશો.

એક જ database, એક જ API, એક જ site – so customer જે order કરે તે તરત જ admin side માં દેખાય. Local જેવું જ behaviour; ફક્ત URLs live હશે.

---

## Resume / Portfolio text

You can write something like:

- **The Shop Hub** (E‑commerce)  
  **Live:** [https://your-frontend.vercel.app](https://your-frontend.vercel.app)  
  **API:** [https://your-api.onrender.com](https://your-api.onrender.com)  
  *Tech: React, Vite, ASP.NET Core Web API, SQL Server, JWT, Cloudinary.*

---

## Important notes

- **Render free tier:** API may sleep after ~15 min inactivity; first request after that can be slow. Good for resume/demo.
- **CORS:** Your API already uses `AllowAnyOrigin()`; for production you can later restrict to your frontend URL only.
- **Secrets:** Never commit `appsettings.json` with real passwords; use environment variables on Render and Vercel/Netlify.
- **Google OAuth:** In Google Cloud Console, add your **live frontend URL** (e.g. `https://your-app.vercel.app`) to authorized origins and redirect URIs so “Sign in with Google” works live.

---

**Faculty reference again:**  
[Lab23 | ASP.NET Core Web API Live](https://github.com/sejalgupta001/AdvanceDotNet/blob/main/Lab23%20%7C%20ASP.NET%20Core%20Web%20API%20Live.md)

Once database, API, and frontend are live, use the two URLs (site + API) in your resume and in project presentations.

---

## Gmail setup (OTP + Login thank-you emails)

Emails (password reset OTP and “thank you for login”) use Gmail SMTP.

1. **Gmail:** Enable **2-Step Verification** (Google Account → Security).
2. **App Password:** Google Account → Security → 2-Step Verification → **App passwords** → Generate for “Mail”. Copy the 16-character password.
3. **API config:** In `ECommerceAPI/appsettings.json`, under **EmailSettings**, set:
   - **FromEmail:** your Gmail (e.g. `yourapp@gmail.com`)
   - **Password:** the 16-character App Password (not your normal Gmail password)
4. **Live (Render):** Do not put the real password in the repo. In Render dashboard → your service → **Environment** → add:
   - `EmailSettings__FromEmail` = your Gmail
   - `EmailSettings__Password` = your App Password  

If these are empty, the API still runs; it just skips sending emails (no error).
