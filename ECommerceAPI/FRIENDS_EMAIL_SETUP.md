# 10 મિત્રોને લિંક મોકલી Sign up + Forgot Password ઇમેઇલ કેવી રીતે આપવી

તમે Vercel app લિંક મિત્રોને મોકલો, મિત્ર **Sign up** કરે ત્યારે **Welcome ઇમેઇલ** અને **Forgot Password** કરે ત્યારે **OTP ઇમેઇલ** એમ બંને **મિત્રના ઈમેઇલ** પર આવે એ માટે નીચેના પગલાં કરો.

---

## શું જરૂરી છે

હમણાં Resend **ફક્ત તમારા ઈમેઇલ** (જે ઈમેઇલથી Resend account બનાવ્યું) પર જ ઇમેઇલ મોકલવા દે છે. **કોઈ પણ મિત્ર** (બીજા ઈમેઇલ) પર ઇમેઇલ મોકલવા માટે Resend પર **domain verify** કરવું જરૂરી છે.

---

## પગલું 1: Domain શું છે?

તમારે **એક domain** જોઈએ જે તમે control કરો – જેમ કે:

- તમે ખરીદેલો domain (જેમ કે `theshophub.com`, `mystore.in`)
- અથવા કોઈ **free domain** (જે સેવા DNS records add કરવા દે)

જો હાલમાં કોઈ domain નથી, તો નીચેમાંથી એક try કરો:

- **Freenom** (free domain – .tk, .ml વગેરે)
- અથવા કોઈ cheap domain (Namecheap, GoDaddy, Google Domains વગેરે)

---

## પગલું 2: Resend પર domain verify કરવું

1. **resend.com** પર login કરો.
2. ડાબી બાજુ **Domains** પર ક્લિક કરો.
3. **Add Domain** ક્લિક કરો.
4. તમારો domain લખો (જેમ કે `theshophub.com`) → **Add**.
5. Resend તમને **2–3 DNS records** બતાવશે (TXT, CNAME, MX વગેરે). એની નકલ કરો.
6. જ્યાંથી તમે domain ખરીદ્યું છે (જેમ કે Namecheap, Freenom) ત્યાં જાઓ → **DNS Settings** / **Manage DNS**.
7. Resend ના બતાવેલા **બધા records** ત્યાં add કરો (Type, Name, Value બરાબર paste કરો).
8. 5–10 મિનિટ રાહ જુઓ, પછી Resend પર **Verify** બટન ક્લિક કરો. Verified થયા પછી green tick દેખાશે.

---

## પગલું 3: Render પર Resend__From બદલવું

1. **Render** → **the-shop-hub-api** → **Environment**.
2. **Resend__From** શોધો. જો ન હોય તો **Add** કરો.
3. **Value** આ રીતે મૂકો (તમારા verified domain પ્રમાણે):

   `The Shop Hub <noreply@તમારો-domain.com>`

   ઉદાહરણ: domain `theshophub.com` હોય તો:

   `The Shop Hub <noreply@theshophub.com>`

4. **Save Changes** કરો. Service પોતે restart થશે.

---

## પગલું 4: Backend deploy

આ repo માં **Welcome email on sign up** add કર્યું છે. જો હજુ push નથી કર્યું તો GitHub પર push કરો જેથી Render પર નવો deploy થાય.

---

## પગલું 5: ટેસ્ટ

1. કોઈ એક **મિત્ર** (અથવા તમે બીજા ઈમેઇલથી) તમારો **Vercel app લિંક** ખોલો.
2. **Sign up** કરો (નવો account).
3. તે ઈમેઇલની **inbox** (અને spam) ચેક કરો → **"Welcome to The Shop Hub!"** ઇમેઇલ આવવી જોઈએ.
4. **Forgot Password** પર જઈ તે જ ઈમેઇલ લખી **Send OTP** કરો → **OTP ઇમેઇલ** આવવી જોઈએ.

બંને ઇમેઇલ આવે = setup યોગ્ય છે. હવે 10 મિત્રોને લિંક મોકલી શકો; સૌને sign up અને forgot password ઇમેઇલ આવશે.

---

**ટૂંક:** મિત્રોને ઇમેઇલ (sign up + OTP) મોકલવા માટે **Resend પર domain verify** કરો, પછી Render પર **Resend__From** માં `The Shop Hub <noreply@તમારો-domain.com>` મૂકો.
