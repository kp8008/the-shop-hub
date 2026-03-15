# OTP અને Login Email માટે Gmail Setup (Render પર)

---

## 0. ઇમેઇલ તમારા Gmail (princekatariyaprince@gmail.com) થી જાય, local જેવું (કોઈ પણ login/forgot કરે ત્યારે OTP/ઇમેઇલ જાય)

જો તમે ઇચ્છો કે **મેલ હંમેશા તમારા admin Gmail થી જાય** અને **કોઈ પણ user** (મિત્ર) login / sign up / forgot password કરે ત્યારે **એને ઇમેઇલ/OTP જાય** (local જેવું), તો **Render પર Gmail use કરો, Resend નહીં.**

**Render પર આ કરો:**

1. **Render** → **the-shop-hub-api** → **Environment**
2. **Resend__ApiKey** હોય તો **delete** કરો (અથવા કદી add ન કર્યું હોય તો ઠીક).  
   - Resend__ApiKey **ન હોય** ત્યારે backend **Gmail** use કરે, ઇમેઇલ **princekatariyaprince@gmail.com** થી જશે.
3. આ **Environment Variables** add/અપડેટ કરો:

   | Key | Value |
   |-----|--------|
   | **EmailSettings__FromEmail** | `princekatariyaprince@gmail.com` |
   | **EmailSettings__FromName** | `The Shop Hub` |
   | **EmailSettings__Password** | તમારો **Gmail App Password** (16 અક્ષર, space વગર) |

   (Gmail App Password કેવી રીતે બનાવવો → નીચે **સેક્શન 2** જુઓ.)

4. **Save Changes** કરો. Service restart થશે.
5. હવે **local જેવું**: કોઈ પણ user sign up / login / forgot password કરે → ઇમેઇલ/OTP **એ user ના ઈમેઇલ** પર જશે, **From** દેખાશે **princekatariyaprince@gmail.com** (The Shop Hub).

**નોંધ:** Render થી Gmail sometimes slow/timeout કરે; જો ઇમેઇલ ન આવે તો Logs ચેક કરો અને સેક્શન 2 માં App Password ફરી verify કરો.

---

## 1. પહેલા ચેક કરો: API સુધી request પહોંચે છે?

Console માં **"timeout of 60000ms exceeded"** અને **"Cannot connect to backend API"** = request **Render API સુધી પહોંચતી નથી**. તે સમયે ઇમેઇલ કોડ ચાલે જ નહીં.

- **Render Dashboard** → **the-shop-hub-api** → **Status** = **Running** હોવું જોઈએ.
- Browser માં ખોલો: `https://the-shop-hub-api.onrender.com` અથવા `https://the-shop-hub-api.onrender.com/swagger`  
  - જો પેજ લોડ થાય = API ચાલે છે. પછી Forgot Password ફરી ટ્રાય કરો (કદાચ cold start પૂરો થયો).
  - જો "Site can't be reached" / timeout = API down અથવા cold start. Render free tier પર 15 મિનિટ idle પછી service sleep પર જાય છે; પહેલો request 50–60+ સેકંડ લગાડી શકે.

જ્યારે API respond કરે (OTP request 200 આવે) પણ **ઇમેઇલ inbox માં ન આવે**, ત્યારે નીચેનું Gmail App Password ચેક/બદલો.

---

## 2. નવો Gmail App Password કેવી રીતે બનાવવો

1. **Google Account** ખોલો: https://myaccount.google.com/  
   (જે Gmail **Render પર EmailSettings__FromEmail** માં લખ્યું છે એ જ account – e.g. princekatariyaprince@gmail.com)

2. **Security** → **2-Step Verification**  
   - જો **OFF** હોય તો **ON** કરો (App Password માટે 2-Step Verification જરૂરી છે).

3. **Security** → **2-Step Verification** → નીચે scroll → **App passwords**  
   - જો "App passwords" દેખાય નહીં તો 2-Step Verification ON કર્યા પછી ફરી ચેક કરો.

4. **App passwords** → **Select app** = "Mail" → **Select device** = "Other" → name લખો: `The Shop Hub Render` → **Generate**.

5. **16 અક્ષરનો password** દેખાશે (જેમ કે `abcd efgh ijkl mnop`).  
   - **Copy** કરો.  
   - **જગ્યા (space) વગર** use કરો: `abcdefghijklmnop`.

6. **Render** → **the-shop-hub-api** → **Environment**  
   - **EmailSettings__Password** = આ **નવો 16-letter App Password** (space વગર) paste કરો.  
   - **Save Changes** કરો. Render service ફરી શરૂ થશે.

7. 2–3 મિનિટ પછી **Forgot Password → Send OTP** અને **Login** ફરી ટ્રાય કરો. Inbox અને **Spam** બંને ચેક કરો.

---

## 3. જો હજુ ઇમેઇલ ન આવે

- **Render** → **Logs** જુઓ.  
  - "Email: sending to ..." પછી "Email sent successfully" = ઇમેઇલ મોકલાઈ; Spam / Promotions ચેક કરો.  
  - "Email failed to ..." + error = તે error મુજબ (e.g. authentication failed = App Password ખોટો; timeout = network/block).

- **Gmail** ક્યારેક cloud server (Render) થી મોકલેલી મેલ block કરે છે. **Resend** વિકલ્પ નીચે પ્રમાણે use કરો.

---

## 4. Resend use કરવું (લાઇવ પર ઇમેઇલ ન આવે ત્યારે)

Local પર Gmail ચાલે પણ **Render (live)** પર OTP/લોગિન ઇમેઇલ ન આવે, કારણ કે Gmail ઘણી વાર datacenter IP થી મેલ block કરે છે. **Resend.com** API થી મોકલવાથી live પર સારી રીતે ચાલે છે.

1. **Resend.com** પર account બનાવો (free tier: 100 emails/day).
2. **API Keys** → **Create API Key** → copy કરો (જેમ કે `re_xxxxxxxx`).
3. **Render** → **the-shop-hub-api** → **Environment** → add:
   - **Resend__ApiKey** = તમારો API key (`re_...`)
   - **Resend__From** = `The Shop Hub <onboarding@resend.dev>`  
     (free tier માં `onboarding@resend.dev` use કરી શકાય; પછી પોતાનો domain verify કરી શકાય)
4. **Save** કરો. Service restart થશે.
5. હવે backend **Resend** થી ઇમેઇલ મોકલશે (Gmail નહીં). Forgot Password OTP અને Login thank-you ફરી ટેસ્ટ કરો.

નોંધ: જો **Resend__ApiKey** set હોય તો Gmail સેટિંગ ignore થાય અને Resend use થશે. Local પર Resend__ApiKey ન રાખો તો local હજુ Gmail થી ચાલશે.

---

## 5. OTP ઇમેઇલ ન આવે, Login ઇમેઇલ આવે? (Resend 483 Forbidden)

જો **login** પછીની ઇમેઇલ આવે પણ **Forgot Password OTP** ન આવે, અને Console માં **Resend API error: Forbidden (483)** / "from address ... domain" દેખાય:

**કારણ:** Resend free tier માં **domain verify ન કર્યું હોય** ત્યારે તમે **ફક્ત તમારા own email** (જે ઈમેઇલથી Resend account બનાવ્યું) **પર જ** મોકલી શકો. બીજા ઈમેઇલ (જેમ કે testcustomer@gmail.com) પર મોકલવા Resend block કરે છે.

**ઉપાય (કોઈ એક):**

- **A) ટેસ્ટ માટે:** Forgot Password માં **તમારો જ ઈમેઇલ** (princekatariyaprince@gmail.com) લખી Send OTP કરો → OTP ઇમેઇલ આવવી જોઈએ. (Login ઇમેઇલ પણ આ ઈમેઇલ પર જ આવે છે.)
- **B) કોઈ પણ user પર OTP મોકલવા:** Resend પર **domain verify** કરો:
  1. **resend.com** → **Domains** → **Add Domain** → તમારો domain (e.g. તમારી siteનો domain) add કરો.
  2. Resend જે DNS records બતાવે (SPF, DKIM, etc.) એ તમારા domain provider માં add કરો → Verify.
  3. **Render** env માં **Resend__From** બદલો, જેમ કે: `The Shop Hub <noreply@yourdomain.com>` (verified domain વાળો ઈમેઇલ).
  4. Redeploy. પછી કોઈ પણ ઈમેઇલ પર OTP મોકલી શકશો.
