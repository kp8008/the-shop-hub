# Vercel પર સાચો deploy (Fix: logo, +91, Google button)

જો localhost પર બધું ઠીક દેખાય પણ **Vercel** પર જૂનું (Vite logo, +1 mobile, નાનું Google button) દેખાય, તો **Vercel Project Settings** ચેક કરો.

## 1. Root Directory (જરૂરી)

આ repo માં frontend **`the-shop-hub`** ફોલ્ડર ની અંદર છે (repo root પર નહીં).

- Vercel → તમારો project **the-shop-hub-ecommerce** → **Settings** → **General**
- **Root Directory** માં લખો: **`the-shop-hub`**
- **Save** કરો.

જો Root Directory ખાલી હોય અથવા ખોટું હોય, તો Vercel repo root થી build કરશે જ્યાં `package.json` નથી, અને જૂનો/ખોટો build ચાલશે.

## 2. Build & Output

- **Build Command:** `npm run build` (default)
- **Output Directory:** `dist` (default)
- **Install Command:** `npm install` (default)

## 3. Redeploy (cache સાફ)

- **Deployments** → ઉપરના deployment પર **⋯** (three dots) → **Redeploy**
- **"Redeploy with existing build cache"** ને **uncheck** કરો (cache સાફ થાય)
- **Redeploy** ક્લિક કરો.

## 4. ચેક કરો

Deploy પૂરો થયા પછી:

- **Favicon:** Tab માં black shopping cart દેખાવો જોઈએ (Vite V નહીં). Hard refresh: **Ctrl+Shift+R**.
- **Register:** Mobile ફીલ્ડમાં **🇮🇳 +91** fixed અને માત્ર 10 અંક.
- **Google button:** મોટું, અને Console માં "Provided button width is invalid: 100%" નહીં.

---

**Summary:** Root Directory = `the-shop-hub` set કરો, પછી **Redeploy without cache** કરો.

---

## Forgot Password OTP / Login thank-you email કામ નથી કરતા?

1. **Vercel પર API URL:**  
   **Settings** → **Environment Variables** → `VITE_API_BASE_URL` = તમારો production API URL  
   (જેમ કે `https://the-shop-hub-api.onrender.com/api`).  
   આ સેટ ન હોય તો live site થી બધા API calls (Forgot Password OTP સહિત) localhost પર જાય અને fail થાય.  
   બદલ્યા પછી **Redeploy** કરો.

2. **Backend (Render) પર Email config:**  
   OTP અને "Thank you for login" email માટે backend પર Gmail સેટિંગ જોઈએ.  
   Render → તમારો API service → **Environment** માં ઉમેરો (જો appsettings ના values use ન થાય તો):  
   - `EmailSettings__FromEmail` = તમારો Gmail  
   - `EmailSettings__Password` = Gmail App Password  
   Save પછી service ફરી શરૂ થશે. Render logs માં "Login thank-you email failed" અથવા "Email not configured" જો તમે જુઓ તો આ config ચેક કરો.

---

## Local ઠીક છે, Vercel પર જૂનું દેખાય? (Verify & fix)

### 1. Vercel પર કયો commit build થયો છે ચેક કરો
- **Vercel** → **Deployments** → ઉપરના (Latest) deployment પર ક્લિક કરો.
- **Source** / **Commit** જુઓ. જો **"Initial commit"** અથવા **687b5f2** જેવું જૂનું commit દેખાય, તો **નવો code build નથી થયો**.
- **Settings** → **Git** → **Connected Repository** ચેક કરો: **kp8008/the-shop-hub** અને branch **main** હોવા જોઈએ.
- જો repo અલગ હોય (અથવા fork), તો આ જ repo **kp8008/the-shop-hub** connect કરો અને **main** branch select કરો.

### 2. Redeploy **without cache**
- **Deployments** → **⋯** (three dots) → **Redeploy**.
- **"Redeploy with existing build cache"** ને **uncheck** કરો → **Redeploy**.

### 3. Live site પર નવો build ચાલે છે ખાતરી કરો
- Browser માં **https://the-shop-hub-ecommerce.vercel.app** ખોલો.
- **Right-click** → **View Page Source** (અથવા Ctrl+U).
- **Ctrl+F** થી શોધો: **shop-hub-deploy** અથવા **icon.svg**
  - જો **`content="v2-icon-91-google"`** અને **`/icon.svg`** દેખાય = નવો build live છે. પછી **Ctrl+Shift+R** (hard refresh) કરો; favicon +91 + Google button update થઈ જશે.
  - જો **vite.svg** દેખાય અથવા **shop-hub-deploy** ન મળે = હજુ જૂનો build serve થાય છે. Step 1–2 ફરી ચેક કરો.

### 4. Browser cache
- **Ctrl+Shift+R** (hard refresh) કરો.
- અથવા **Incognito/Private** window માં site ખોલીને ચેક કરો.
