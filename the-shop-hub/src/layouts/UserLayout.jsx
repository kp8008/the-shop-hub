import { Outlet } from 'react-router-dom'
import { GuestSignUpProvider } from '../context/GuestSignUpContext'
import Navbar from '../components/user/Navbar'
import Footer from '../components/user/Footer'

const UserLayout = () => {
  return (
    <GuestSignUpProvider>
      <div className="min-h-screen flex flex-col">
        <Navbar />
        <main className="flex-grow">
          <Outlet />
        </main>
        <Footer />
      </div>
    </GuestSignUpProvider>
  )
}

export default UserLayout
