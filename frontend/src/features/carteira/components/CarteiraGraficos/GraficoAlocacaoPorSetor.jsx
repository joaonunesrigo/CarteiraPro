import { Cell, Pie, PieChart, ResponsiveContainer, Tooltip } from 'recharts'
import { formatarMoeda } from '../../../../utils/format'

export function GraficoAlocacaoPorSetor({ dados }) {
  return (
    <ResponsiveContainer width="100%" height={280}>
      <PieChart>
        <Pie
          data={dados}
          dataKey="valor"
          nameKey="setor"
          cx="50%"
          cy="50%"
          innerRadius={56}
          outerRadius={96}
          paddingAngle={2}
          label={({ percentual }) => `${percentual}%`}
          labelLine={false}
        >
          {dados.map((item) => (
            <Cell key={item.setor} fill={item.cor} stroke="transparent" />
          ))}
        </Pie>
        <Tooltip
          content={({ active, payload }) => {
            if (!active || !payload?.length) return null
            const item = payload[0].payload
            return (
              <div className="min-w-[180px] rounded-lg border border-slate-700 bg-slate-900 px-3 py-2 text-sm shadow-lg">
                <p className="font-medium text-white">{item.setor}</p>
                <p className="text-slate-300">{formatarMoeda(item.valor)}</p>
                <p className="text-slate-400">{item.percentual}% do grupo</p>
                {item.ativos?.length > 0 && (
                  <ul className="mt-2 space-y-0.5 border-t border-slate-700 pt-2 text-xs text-slate-300">
                    {item.ativos.map((ativo) => (
                      <li key={ativo.ticker} className="flex items-center justify-between gap-3">
                        <span className="font-medium text-slate-200">{ativo.ticker}</span>
                        <span className="text-slate-400">{formatarMoeda(ativo.valor)}</span>
                      </li>
                    ))}
                  </ul>
                )}
              </div>
            )
          }}
        />
      </PieChart>
    </ResponsiveContainer>
  )
}
