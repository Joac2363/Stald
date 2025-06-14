interface NavbarElementProps {
    content: string
    onClick: () => void,
}

export default function NavbarElement({content, onClick}: NavbarElementProps) {
    return (
        <button onClick={onClick} className="text-xs px-2 font-mono font-semibold">{content}</button>
    )
}