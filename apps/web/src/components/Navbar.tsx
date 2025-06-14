import Calendar from "../pages/Calendar";
import Dashboard from "../pages/Dashboard";
import Horses from "../pages/Horses";
import Posts from "../pages/Posts";
import NavbarElement from "./NavbarElement";
import type { JSX } from "react";

interface NavbarProps {
    onChangePage: (page: JSX.Element) => void
}

export default function Navbar({onChangePage} : NavbarProps) {
    return (
            <div className="w-full h-fit flex justify-center p-2 bg-gray-100 sticky">
                <NavbarElement onClick={() => onChangePage(<Dashboard/>)} content="Dashboard"/>
                <NavbarElement onClick={() => onChangePage(<Calendar/>)} content="Calendar" />
                <NavbarElement onClick={() => onChangePage(<Posts/>)} content="Posts" />
                <NavbarElement onClick={() => onChangePage(<Horses/>)} content="Horses" />
            </div>
    )
}