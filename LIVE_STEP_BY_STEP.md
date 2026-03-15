# The Shop Hub – Live કરવા સંપૂર્ણ Step-by-Step Guide

આ ગાઇડ પ્રમાણે કરશો તો તમારો project live થઈ જશે. એક પછી એક step follow કરો.

---

## Step 1 — Database online કરો (Somee.com)

### 1.1 Account બનાવો
1. Browser માં ખોલો: **https://www.somee.com**
2. **Register** ક્લિક કરો → તમારો **email** અને **password** લખો → Sign up.
3. Email માં આવેલો **verification link** ખોલો.

### 1.2 Database create કરો
1. Somee પર **Login** કરો.
2. Dashboard માં **SQL Server** section શોધો.
3. **Create Database** ક્લિક કરો.
4. **Database name** લખો (જેમ કે `ECommerceDb`).
5. Create કરો. પછી તમને આ મળશે – **આ સંભાળી લો:**
   - **Server name** (જેમ કે `SQL5011.somee.com`)
   - **Database name**
   - **Username**
   - **Password**

### 1.3 તમારો local database Somee પર લાવો
1. **SQL Server Management Studio (SSMS)** ખોલો.
2. **Connect** કરો: Server name = Somee નો server (જેમ કે `SQL5011.somee.com`), Authentication = **SQL Server Authentication**, Login = Somee username, Password = Somee password.
3. Connect થયા પછી:
   - **Option A:** તમારા local database પર Right click → **Tasks** → **Generate Scripts** → સંપૂર્ણ DB script generate કરો → Somee પર નવી database select કરીને તે script **Execute** કરો.
   - **Option B:** જો તમારી પાસે **.bak** file હોય તો Restore use કરી શકો (Somee support કરે તો).

### 1.4 Connection string નોંધો
Somee થી મળેલી details થી connection string આ રીતે હશે:

```
Server=તમારોServerName.somee.com;Database=તમારોDBName;User Id=તમારોUsername;Password=તમારોPassword;TrustServerCertificate=True;
```

આ string **copy** કરી લો – Step 3 માં Render પર મૂકવાની છે.

---

## Step 2 — Code GitHub પર મોકલો

### 2.1 GitHub repo બનાવો
1. **https://github.com** ખોલો → Login.
2. **New** (અથવા +) ક્લિક કરો → **New repository**.
3. Repository name: `the-shop-hub` (અથવા જે પણ નામ ગમે).
4. **Private** અથવા **Public** પસંદ કરો → **Create repository**.

### 2.2 Local project push કરો
1. તમારો **ADN** folder (જેમાં ECommerceAPI, the-shop-hub, Dockerfile સૌ છે) ખોલો.
2. **Git** install હોવો જોઈએ. Terminal/PowerShell ખોલો, ADN folder માં જાઓ:
   ```bash
   cd d:\SEM-6\ADN
   git init
   git add .
   git commit -m "Initial commit - The Shop Hub"
   git branch -M main
   git remote add origin https://github.com/તમારોUsername/તમારોRepoName.git
   git push -u origin main
   ```
3. `તમારોUsername` અને `તમારોRepoName` ની જગ્યાએ GitHub username અને repo name મૂકો.
4. જો password માંગે તો GitHub **Personal Access Token** use કરો (password નહીં).

**અથવા** Visual Studio માં: **Git** → **Create Git Repository** → GitHub connect કરો → same repo select → **Create and Push**.

⚠️ **Important:** `appsettings.json` માં **real passwords commit ન કરો**. Render પર આપણે Environment Variables માં મૂકશું. જો already commit થયું હોય તો Render પર જ env vars use કરશો, repo માં password change ન કરશો.

---

## Step 3 — Backend API live કરો (Render)

### 3.1 Render પર account અને service
1. **https://dashboard.render.com** ખોલો → **Get Started** → **GitHub** થી sign up / login.
2. **New +** ક્લિક કરો → **Web Service** પસંદ કરો.
3. **Connect a repository** – તમારો **the-shop-hub** (અથવા જે repo માં ADN push કર્યો) select કરો → **Connect**.

### 3.2 Settings ભરો
- **Name:** `the-shop-hub-api` (અથવા કંઈ પણ)
- **Region:** India નજીકનો select કરો.
- **Branch:** `main`
- **Runtime:** **Docker**
- **Dockerfile Path:** `Dockerfile` (ખાલી નહીં – exactly `Dockerfile`)
- **Instance Type:** **Free**

### 3.3 Environment Variables add કરો
**Environment** section માં **Add Environment Variable** ક્લિક કરીને એક એક કરીને આ add કરો (values તમારી લખો):

| Key | Value (example – તમારી values મૂકો) |
|-----|--------------------------------------|
| `ConnectionStrings__Default` | Step 1.4 નો connection string |
| `JwtSettings__SecretKey` | કોઈ પણ 32+ character long secret (જેમ કે `MySuperSecretKeyForTheShopHub2024!`) |
| `EmailSettings__FromEmail` | તમારો Gmail (જેમ કે adm@gmail.com) |
| `EmailSettings__Password` | Gmail App Password (16 અક્ષર, space વગર) |
| `CloudinarySettings__CloudName` | તમારો Cloudinary cloud name |
| `CloudinarySettings__ApiKey` | Cloudinary API key |
| `CloudinarySettings__ApiSecret` | Cloudinary API secret |

### 3.4 Deploy
1. **Create Web Service** ક્લિક કરો.
2. Render build કરશે (2–5 min લાગી શકે).
3. **Logs** માં error ન આવે તો deploy success. ઉપર **URL** દેખાશે, જેમ કે:  
   **https://the-shop-hub-api.onrender.com**

### 3.5 Test
Browser માં તે URL ખોલો. **Swagger** page દેખાશે = API live છે.

**આ API URL નોંધી લો** – આગળ frontend માં મૂકવાનો છે (પાછળ `/api` ઉમેરશો).

---

## Step 4 — Frontend live કરો (Vercel)

### 4.1 Vercel account અને project
1. **https://vercel.com** ખોલો → **Sign Up** → **Continue with GitHub**.
2. **Add New** → **Project**.
3. **Import** તમારો **same GitHub repo** (જેમાં ADN છે).
4. **Import** ક્લિક કરો.

### 4.2 Project settings
- **Project Name:** the-shop-hub (અથવા જે ગમે)
- **Root Directory:** **Edit** ક્લિક કરો → **the-shop-hub** select કરો (ફક્ત frontend folder, ADN નહીં).
- **Framework Preset:** **Vite** (auto detect થઈ શકે).
- **Build Command:** `npm run build` (default)
- **Output Directory:** `dist` (default)

### 4.3 Environment Variables
**Environment Variables** section માં add કરો:

| Name | Value |
|------|--------|
| `VITE_API_BASE_URL` | `https://તમારોRenderAPIURL.onrender.com/api` (Step 3.4 નો URL + `/api`) |
| `VITE_GOOGLE_CLIENT_ID` | તમારો Google OAuth Client ID |

Example: જો API URL `https://the-shop-hub-api.onrender.com` હોય તો  
`VITE_API_BASE_URL` = `https://the-shop-hub-api.onrender.com/api`

### 4.4 Deploy
1. **Deploy** ક્લિક કરો.
2. Build complete થયા પછી તમને **site URL** મળશે, જેમ કે:  
   **https://the-shop-hub-xxx.vercel.app**

આ જ **link** તમે resume માં અને friends ને મોકલશો.

---

## Step 5 — Google OAuth (live site માટે)

જો "Continue with Google" live પર કામ કરવું હોય તો:

1. **https://console.cloud.google.com** ખોલો.
2. તમારો project select કરો → **APIs & Services** → **Credentials**.
3. OAuth 2.0 Client ID (Web application) edit કરો.
4. **Authorized JavaScript origins** માં add કરો: `https://તમારોVercelURL.vercel.app`
5. **Authorized redirect URIs** માં પણ same origin add કરો (જો required હોય).
6. Save કરો.

---

## Step 6 — Final check

1. **Frontend URL** ખોલો (Vercel વાળો).
2. **Sign Up** → નવો customer બનાવો → **Login** → ઈમેઇલ આવે ("Thank you for login...").
3. **Forgot password** ટ્રાય કરો → OTP ઈમેઇલ પર આવે.
4. **Admin** તરીકે login કરો (તમારો adm@gmail.com) → **Orders** જુઓ.

બધું કામ કરે = **project live success.**

---

## Resume / Portfolio માં શું લખશો

- **The Shop Hub** (E‑commerce)  
  **Live:** https://તમારો-vercel-url.vercel.app  
  **API:** https://તમારો-render-url.onrender.com  
  *Tech: React, Vite, ASP.NET Core Web API, SQL Server, JWT, Gmail SMTP, Cloudinary.*

---

## સારાંશ – ક્રમ

| # | શું | ક્યાં |
|---|-----|-------|
| 1 | Database online | Somee.com |
| 2 | Code GitHub પર | GitHub repo |
| 3 | API live | Render (Docker) |
| 4 | Website live | Vercel (the-shop-hub folder) |
| 5 | Google OAuth live URL add | Google Cloud Console |
| 6 | Test & resume માં link | — |

કોઈ step પર અટકો તો **LIVE_DEPLOYMENT.md** માં વધુ detail છે. Faculty reference: [Lab23 \| ASP.NET Core Web API Live](https://github.com/sejalgupta001/AdvanceDotNet/blob/main/Lab23%20%7C%20ASP.NET%20Core%20Web%20API%20Live.md)
