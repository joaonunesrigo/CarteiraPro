import { Area, AreaChart, CartesianGrid, Line, ResponsiveContainer, Tooltip, XAxis, YAxis } from 'recharts'
import { ChartTooltip } from '../../../../components/ChartTooltip'
import { Panel } from '../../../../components/Panel'
import { formatarMoeda } from '../../../../utils/format'

function formatarEixoMoeda(valor) {
  if (Math.abs(valor) >= 1_000_000) return `R$ ${(valor / 1_000_000).toFixed(1)}M`
  if (Math.abs(valor) >= 1_000) return `R$ ${(valor / 1_000).toFixed(0)}k`
  return formatarMoeda(valor)
}

function SeletorPeriodo({ periodos, periodoSelecionado, selecionarPeriodo }) {
  return (
    <div className="flex flex-wrap gap-1 rounded-lg border border-slate-700 bg-slate-900/60 p-1">
      {periodos.map((periodo) => {
        const ativo = periodo.id === periodoSelecionado
        return (
          <button
            key={periodo.id}
            type="button"
            onClick={() => selecionarPeriodo(periodo.id)}
            className={`rounded-md px-3 py-1 text-xs font-medium transition ${
              ativo ? 'bg-violet-600 text-white' : 'text-slate-300 hover:bg-slate-800 hover:text-white'
            }`}
          >
            {periodo.rotulo}
          </button>
        )
      })}
    </div>
  )
}

function VariacaoResumo({ pontos }) {
  if (pontos.length < 2) return null

  const inicial = pontos[0]
  const final = pontos[pontos.length - 1]
  const variacaoReais = final.patrimonio - inicial.patrimonio
  const variacaoPercentual = inicial.patrimonio > 0 ? (variacaoReais / inicial.patrimonio) * 100 : 0
  const positivo = variacaoReais >= 0

  return (
    <div className="flex flex-wrap items-baseline gap-x-4 gap-y-1 text-sm">
      <span className="text-slate-400">No período:</span>
      <span className={`tabular-nums font-medium ${positivo ? 'text-emerald-400' : 'text-red-400'}`}>
        {positivo ? '+' : ''}
        {formatarMoeda(variacaoReais)}
      </span>
      <span className={`tabular-nums text-xs ${positivo ? 'text-emerald-400' : 'text-red-400'}`}>
        ({positivo ? '+' : ''}
        {variacaoPercentual.toFixed(2)}%)
      </span>
    </div>
  )
}

export function GraficoDesempenhoCarteira({
  periodos,
  periodoSelecionado,
  selecionarPeriodo,
  pontos,
  tickersSemHistorico,
  carregando,
  erro,
}) {
  return (
    <Panel
      titulo="Desempenho da carteira"
      acao={<SeletorPeriodo periodos={periodos} periodoSelecionado={periodoSelecionado} selecionarPeriodo={selecionarPeriodo} />}
    >
      <div className="space-y-3 p-4">
        {carregando ? (
          <p className="py-12 text-center text-sm text-slate-400">Carregando histórico...</p>
        ) : erro ? (
          <p className="py-12 text-center text-sm text-red-400">{erro}</p>
        ) : pontos.length === 0 ? (
          <p className="py-12 text-center text-sm text-slate-400">Sem dados suficientes para o período selecionado.</p>
        ) : (
          <>
            <VariacaoResumo pontos={pontos} />

            <ResponsiveContainer width="100%" height={300}>
              <AreaChart data={pontos} margin={{ top: 8, right: 16, left: 0, bottom: 0 }}>
                <defs>
                  <linearGradient id="patrimonioGradient" x1="0" y1="0" x2="0" y2="1">
                    <stop offset="5%" stopColor="#a78bfa" stopOpacity={0.45} />
                    <stop offset="95%" stopColor="#a78bfa" stopOpacity={0} />
                  </linearGradient>
                </defs>
                <CartesianGrid strokeDasharray="3 3" stroke="#334155" vertical={false} />
                <XAxis dataKey="rotulo" stroke="#94a3b8" tick={{ fill: '#94a3b8', fontSize: 12 }} axisLine={{ stroke: '#475569' }} />
                <YAxis
                  stroke="#94a3b8"
                  tick={{ fill: '#94a3b8', fontSize: 12 }}
                  axisLine={{ stroke: '#475569' }}
                  tickFormatter={formatarEixoMoeda}
                  width={64}
                />
                <Tooltip
                  content={({ active, payload, label }) => <ChartTooltip active={active} payload={payload} label={label} tipo="moeda" />}
                />
                <Area
                  type="monotone"
                  dataKey="patrimonio"
                  name="Patrimônio"
                  stroke="#a78bfa"
                  strokeWidth={2}
                  fill="url(#patrimonioGradient)"
                  activeDot={{ r: 4 }}
                />
                <Line
                  type="monotone"
                  dataKey="valorInvestido"
                  name="Investido"
                  stroke="#64748b"
                  strokeWidth={1.5}
                  strokeDasharray="4 4"
                  dot={false}
                  activeDot={false}
                />
              </AreaChart>
            </ResponsiveContainer>

            <div className="flex flex-wrap items-center gap-4 text-xs text-slate-400">
              <span className="flex items-center gap-2">
                <span className="h-2.5 w-3 rounded-sm bg-violet-400" />
                Patrimônio
              </span>
              <span className="flex items-center gap-2">
                <span className="h-0.5 w-3 bg-slate-500" />
                Investido
              </span>
            </div>

            <p className="text-xs leading-relaxed text-slate-500">
              O desempenho histórico considera as operações registradas no sistema. Ativos importados como posição inicial passam a contar a
              partir da data de importação.
            </p>

            {tickersSemHistorico.length > 0 && (
              <p className="text-xs text-amber-400/90">
                Sem cotação histórica para: {tickersSemHistorico.join(', ')}. Esses ativos entram pelo preço médio nesse período.
              </p>
            )}
          </>
        )}
      </div>
    </Panel>
  )
}
