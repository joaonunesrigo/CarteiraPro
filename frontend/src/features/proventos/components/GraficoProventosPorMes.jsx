import { Bar, BarChart, CartesianGrid, ResponsiveContainer, Tooltip, XAxis, YAxis } from 'recharts'
import { ChartTooltip } from '../../../components/ChartTooltip'

const COR_PROVENTOS = '#10b981'

function formatarEixoMoeda(valor) {
  if (valor >= 1000) return `R$ ${(valor / 1000).toFixed(0)}k`
  return `R$ ${valor}`
}

function formatarMes(mes) {
  const [ano, mesNumero] = mes.split('-').map(Number)
  const data = new Date(Date.UTC(ano, mesNumero - 1, 1))

  return data.toLocaleDateString('pt-BR', {
    month: 'short',
    year: '2-digit',
    timeZone: 'UTC',
  })
}

export function GraficoProventosPorMes({ dados }) {
  const dadosFormatados = dados.map((item) => ({
    mes: item.mes,
    rotulo: formatarMes(item.mes),
    total: item.total,
  }))

  return (
    <ResponsiveContainer width="100%" height={280}>
      <BarChart data={dadosFormatados} margin={{ top: 8, right: 8, left: 0, bottom: 0 }}>
        <CartesianGrid strokeDasharray="3 3" stroke="#334155" vertical={false} />
        <XAxis dataKey="rotulo" stroke="#94a3b8" tick={{ fill: '#94a3b8', fontSize: 12 }} axisLine={{ stroke: '#475569' }} />
        <YAxis
          stroke="#94a3b8"
          tick={{ fill: '#94a3b8', fontSize: 12 }}
          axisLine={{ stroke: '#475569' }}
          tickFormatter={formatarEixoMoeda}
        />
        <Tooltip content={({ active, payload, label }) => <ChartTooltip active={active} payload={payload} label={label} />} />
        <Bar dataKey="total" name="Recebido" fill={COR_PROVENTOS} radius={[4, 4, 0, 0]} />
      </BarChart>
    </ResponsiveContainer>
  )
}
