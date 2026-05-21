import { cn } from '../../utils/cn'

export function Panel({ titulo, acao, children, className }) {
  return (
    <section className={cn('overflow-hidden rounded-xl border border-slate-800', className)}>
      {titulo && (
        <div className="flex items-center justify-between gap-4 border-b border-slate-800 px-4 py-3">
          <h2 className="text-lg font-medium text-white">{titulo}</h2>
          {acao}
        </div>
      )}
      {children}
    </section>
  )
}
