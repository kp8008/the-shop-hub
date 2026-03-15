import { Link } from 'react-router-dom'
import { Facebook, Twitter, Instagram, Mail, Phone, MapPin } from 'lucide-react'

const Footer = () => {
  return (
    <footer id="footer" className="mt-auto bg-kaira-light border-t border-gray-200">
      <div className="max-w-[1800px] mx-auto px-4 md:px-6 py-12 md:py-16">
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-10 lg:gap-8">
          {/* About / Intro - Kaira footer-menu-001 */}
          <div>
            <div className="mb-4">
              <Link to="/" className="font-marcellus text-xl text-[#111] tracking-wide">
                The Shop Hub
              </Link>
            </div>
            <p className="text-sm text-gray-600 font-jost leading-relaxed mb-6 max-w-xs">
              Your one-stop destination for quality products, great prices, and exceptional service.
            </p>
            <div className="flex flex-wrap gap-3">
              <a href="#" className="text-gray-500 hover:text-[#111] transition-colors" aria-label="Facebook">
                <Facebook className="w-5 h-5" />
              </a>
              <a href="#" className="text-gray-500 hover:text-[#111] transition-colors" aria-label="Twitter">
                <Twitter className="w-5 h-5" />
              </a>
              <a href="#" className="text-gray-500 hover:text-[#111] transition-colors" aria-label="Instagram">
                <Instagram className="w-5 h-5" />
              </a>
            </div>
          </div>

          {/* Quick Links - Kaira footer-menu-002 */}
          <div>
            <h5 className="font-marcellus text-[#111] text-sm uppercase tracking-[0.12em] mb-6">
              Quick Links
            </h5>
            <ul className="space-y-4 text-sm font-jost uppercase tracking-[0.06em] text-gray-600">
              <li>
                <Link to="/" className="nav-link-kaira text-gray-600 hover:text-[#111] inline-block">
                  Home
                </Link>
              </li>
              <li>
                <Link to="/products" className="nav-link-kaira text-gray-600 hover:text-[#111] inline-block">
                  Shop
                </Link>
              </li>
              <li>
                <Link to="/about" className="nav-link-kaira text-gray-600 hover:text-[#111] inline-block">
                  About Us
                </Link>
              </li>
              <li>
                <Link to="/contact" className="nav-link-kaira text-gray-600 hover:text-[#111] inline-block">
                  Contact
                </Link>
              </li>
              <li>
                <Link to="/faq" className="nav-link-kaira text-gray-600 hover:text-[#111] inline-block">
                  FAQ
                </Link>
              </li>
            </ul>
          </div>

          {/* Help & Info - Kaira footer-menu-003 */}
          <div>
            <h5 className="font-marcellus text-[#111] text-sm uppercase tracking-[0.12em] mb-6">
              Help & Info
            </h5>
            <ul className="space-y-4 text-sm font-jost uppercase tracking-[0.06em] text-gray-600">
              <li>
                <Link to="/shipping" className="nav-link-kaira text-gray-600 hover:text-[#111] inline-block">
                  Shipping Info
                </Link>
              </li>
              <li>
                <Link to="/returns" className="nav-link-kaira text-gray-600 hover:text-[#111] inline-block">
                  Returns
                </Link>
              </li>
              <li>
                <Link to="/contact" className="nav-link-kaira text-gray-600 hover:text-[#111] inline-block">
                  Contact Us
                </Link>
              </li>
            </ul>
          </div>

          {/* Contact Us - Kaira footer-menu-004 */}
          <div>
            <h5 className="font-marcellus text-[#111] text-sm uppercase tracking-[0.12em] mb-6">
              Contact Us
            </h5>
            <div className="space-y-4 text-sm font-jost text-gray-600">
              <p className="leading-relaxed">
                Questions or suggestions?{' '}
                <a href="mailto:support@theshophub.com" className="nav-link-kaira text-gray-600 hover:text-[#111]">
                  support@theshophub.com
                </a>
              </p>
              <ul className="space-y-3">
                <li className="flex items-start gap-2">
                  <MapPin className="w-5 h-5 flex-shrink-0 mt-0.5 text-kaira" />
                  <span>Gondal, Gujarat 360311</span>
                </li>
                <li className="flex items-center gap-2">
                  <Phone className="w-5 h-5 flex-shrink-0 text-kaira" />
                  <a href="tel:+919999777701" className="nav-link-kaira text-gray-600 hover:text-[#111]">
                    +91 9999777701
                  </a>
                </li>
                <li className="flex items-center gap-2">
                  <Mail className="w-5 h-5 flex-shrink-0 text-kaira" />
                  <a href="mailto:support@theshophub.com" className="nav-link-kaira text-gray-600 hover:text-[#111]">
                    support@theshophub.com
                  </a>
                </li>
              </ul>
            </div>
          </div>
        </div>
      </div>

      {/* Bottom bar - Kaira border-top */}
      <div className="border-t border-gray-200 py-4 md:py-5">
        <div className="max-w-[1800px] mx-auto px-4 md:px-6">
          <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-4 text-center md:text-left">
            <p className="text-xs md:text-sm text-gray-500 font-jost">
              © {new Date().getFullYear()} The Shop Hub. All rights reserved.
            </p>
          </div>
        </div>
      </div>
    </footer>
  )
}

export default Footer
