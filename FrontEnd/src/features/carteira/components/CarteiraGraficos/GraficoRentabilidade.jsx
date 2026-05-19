import {
  Bar,
  BarChart,
  CartesianGrid,
  Cell,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from 'recharts'
import { ChartTooltip } from '../../../../components/ChartTooltip'
import { COR_NEGATIVA, COR_POSITIVA } from '../../constants/graficos.constants'

export function GraficoRentabilidade({ dados }) {
  return (
    <ResponsiveContainer width="100%" height={280}>
      <BarChart
        data={dados}
        layout="vertical"
        margin={{ top: 8, right: 16, left: 8, bottom: 0 }}
      >
        <CartesianGrid strokeDasharray="3 3" stroke="#334155" horizontal={false} />
        <XAxis
          type="number"
          stroke="#94a3b8"
          tick={{ fill: '#94a3b8', fontSize: 12 }}
          axisLine={{ stroke: '#475569' }}
          tickFormatter={(valor) => `${valor}%`}
        />
        <YAxis
          type="category"
          dataKey="ticker"
          stroke="#94a3b8"
          tick={{ fill: '#94a3b8', fontSize: 12 }}
          axisLine={{ stroke: '#475569' }}
          width={56}
        />
        <Tooltip
          content={({ active, payload, label }) => (
            <ChartTooltip
              active={active}
              payload={payload}
              label={label}
              tipo="percentual"
            />
          )}
        />
        <Bar dataKey="percentual" name="Rentabilidade" radius={[0, 4, 4, 0]}>
          {dados.map((item) => (
            <Cell
              key={item.ticker}
              fill={item.positivo ? COR_POSITIVA : COR_NEGATIVA}
            />
          ))}
        </Bar>
      </BarChart>
    </ResponsiveContainer>
  )
}
