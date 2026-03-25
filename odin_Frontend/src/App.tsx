// src/App.tsx
import TacticalMap from './components/TacticalMap';

function App() {
  return (
    <div className="flex h-screen w-screen overflow-hidden bg-slate-900">
      {/* Sidebar placeholder for later */}
      <div className="w-80 bg-slate-800 border-r border-slate-700 shadow-xl z-10 flex flex-col hidden md:flex">
        <div className="p-6 border-b border-slate-700">
          <h1 className="text-2xl font-bold text-emerald-400 tracking-wider">ODIN</h1>
          <p className="text-xs text-slate-400 uppercase tracking-widest mt-1">Tactical Overwatch</p>
        </div>
        <div className="p-6 flex-1 text-slate-300">
          <p className="text-sm">Live telemetry feed standing by...</p>
        </div>
      </div>

      {/* Main Map Area */}
      <main className="flex-1 relative">
        <TacticalMap />
      </main>
    </div>
  );
}

export default App;