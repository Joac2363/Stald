import { useState } from "react"
import Navbar from "./components/Navbar"
import Dashboard from "./pages/Dashboard"

function App() {
  const [currentPage, setCurrentPage] = useState(<Dashboard/>)
  

  return (
    <div className="font-mono font-semibold h-screen flex flex-col">
    <Navbar onChangePage={setCurrentPage}/>
    {currentPage}
    </div>
  )
}

export default App
