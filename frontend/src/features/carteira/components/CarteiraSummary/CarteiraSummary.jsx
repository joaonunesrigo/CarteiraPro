import { StatCard } from '../../../../components/StatCard'

function formatarAtualizacao(dataIso) {
  if (!dataIso) return null

  return new Intl.DateTimeFormat('pt-BR', {
    dateStyle: 'short',
    timeStyle: 'short',
  }).format(new Date(dataIso))
}

export function CarteiraSummary({ cartoes, cotacaoAtualizadaEm }) {
  if (!cartoes.length) return null
  const atualizacaoFormatada = formatarAtualizacao(cotacaoAtualizadaEm)

  return (
    <section className="space-y-3">
      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
        {cartoes.map((cartao) => (
          <StatCard
            key={cartao.id}
            rotulo={cartao.rotulo}
            valor={cartao.valorFormatado}
            comSinal={cartao.comSinal}
            positivo={cartao.positivo}
          />
        ))}
      </div>

      {atualizacaoFormatada && (
        <p className="text-sm text-slate-500">
          Última cotação Brapi: {atualizacaoFormatada}. Atualização automática a cada 30 minutos durante o pregão.
        </p>
      )}
    </section>
  )
}
