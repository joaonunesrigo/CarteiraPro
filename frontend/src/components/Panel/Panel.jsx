import { cn } from '../../utils/cn'

export function Panel({ titulo, children, className }) {
  return (
    <section
      className={cn('overflow-hidden rounded-xl border border-slate-800', className)}
    >
      {titulo && (
        <div className="border-b border-slate-800 px-4 py-3">
          <h2 className="text-lg font-medium text-white">{titulo}</h2>
        </div>
      )}
      {children}
    </section>
  )
}
