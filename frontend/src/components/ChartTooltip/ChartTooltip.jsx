import { formatarMoeda, formatarPercentual } from '../../utils/format'

export function ChartTooltip({ active, payload, label, tipo = 'moeda' }) {
  if (!active || !payload?.length) return null

  return (
    <div className="rounded-lg border border-slate-700 bg-slate-900 px-3 py-2 text-sm shadow-lg">
      {label && <p className="mb-1 font-medium text-white">{label}</p>}
      {payload.map((item) => (
        <p key={item.name} className="text-slate-300">
          <span style={{ color: item.color || item.payload?.cor }}>
            {item.name}:{' '}
          </span>
          {tipo === 'percentual'
            ? formatarPercentual(item.value)
            : formatarMoeda(item.value)}
        </p>
      ))}
    </div>
  )
}
