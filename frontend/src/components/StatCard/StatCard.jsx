export function StatCard({ rotulo, valor, comSinal, positivo }) {
  return (
    <article className="rounded-xl border border-slate-800 bg-slate-900/50 p-5">
      <p className="text-sm text-slate-400">{rotulo}</p>
      <p
        className={`mt-2 text-2xl font-semibold tabular-nums ${comSinal ? (positivo ? 'text-emerald-400' : 'text-red-400') : 'text-white'}`}
      >
        {valor}
      </p>
    </article>
  )
}
