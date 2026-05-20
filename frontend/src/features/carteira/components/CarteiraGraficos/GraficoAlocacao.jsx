import { Cell, Pie, PieChart, ResponsiveContainer, Tooltip } from 'recharts'
import { formatarMoeda } from '../../../../utils/format'

export function GraficoAlocacao({ dados }) {
  return (
    <ResponsiveContainer width="100%" height={280}>
      <PieChart>
        <Pie
          data={dados}
          dataKey="valor"
          nameKey="ticker"
          cx="50%"
          cy="50%"
          innerRadius={56}
          outerRadius={96}
          paddingAngle={2}
        >
          {dados.map((item) => (
            <Cell key={item.ticker} fill={item.cor} stroke="transparent" />
          ))}
        </Pie>
        <Tooltip
          content={({ active, payload }) => {
            if (!active || !payload?.length) return null
            const item = payload[0].payload
            return (
              <div className="rounded-lg border border-slate-700 bg-slate-900 px-3 py-2 text-sm shadow-lg">
                <p className="font-medium text-white">{item.ticker}</p>
                <p className="text-slate-300">{formatarMoeda(item.valor)}</p>
                <p className="text-slate-400">{item.percentual}% da carteira</p>
              </div>
            )
          }}
        />
      </PieChart>
    </ResponsiveContainer>
  )
}
