import {
  Bar,
  BarChart,
  CartesianGrid,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from 'recharts'
import { ChartTooltip } from '../../../../components/ChartTooltip'
import { COR_ATUAL, COR_INVESTIDO } from '../../constants/graficos.constants'

function formatarEixoMoeda(valor) {
  if (valor >= 1000) return `R$ ${(valor / 1000).toFixed(0)}k`
  return `R$ ${valor}`
}

export function GraficoValores({ dados }) {
  return (
    <ResponsiveContainer width="100%" height={280}>
      <BarChart data={dados} margin={{ top: 8, right: 8, left: 0, bottom: 0 }}>
        <CartesianGrid strokeDasharray="3 3" stroke="#334155" vertical={false} />
        <XAxis
          dataKey="ticker"
          stroke="#94a3b8"
          tick={{ fill: '#94a3b8', fontSize: 12 }}
          axisLine={{ stroke: '#475569' }}
        />
        <YAxis
          stroke="#94a3b8"
          tick={{ fill: '#94a3b8', fontSize: 12 }}
          axisLine={{ stroke: '#475569' }}
          tickFormatter={formatarEixoMoeda}
        />
        <Tooltip
          content={({ active, payload, label }) => (
            <ChartTooltip active={active} payload={payload} label={label} />
          )}
        />
        <Bar
          dataKey="investido"
          name="Investido"
          fill={COR_INVESTIDO}
          radius={[4, 4, 0, 0]}
        />
        <Bar dataKey="atual" name="Atual" fill={COR_ATUAL} radius={[4, 4, 0, 0]} />
      </BarChart>
    </ResponsiveContainer>
  )
}
