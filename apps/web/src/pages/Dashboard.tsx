export default function Dashboard() {
    return (
        <div className="h-full flex flex-row">
            <div className="w-1/2 flex flex-col justify-between p-2">
                <h1 className="text-4xl">Velkommen!</h1>
                <div className="border-1 h-93/100 border-gray-300 rounded" >b1</div>
            </div>
            <div className="w-1/2 flex flex-col justify-between p-2">
                <div className="border-1 h-60/101 border-gray-300 rounded ">b1</div>
                <div className="border-1 h-40/101 border-gray-300 rounded ">b2</div>
            </div>
        </div>
    )
}