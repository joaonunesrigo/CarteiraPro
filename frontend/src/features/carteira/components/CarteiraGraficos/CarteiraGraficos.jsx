import { EmptyState } from '../../../../components/EmptyState'
import { Panel } from '../../../../components/Panel'
import { GraficoAlocacao } from './GraficoAlocacao'
import { GraficoAlocacaoPorSetor } from './GraficoAlocacaoPorSetor'
import { GraficoAlocacaoPorTipo } from './GraficoAlocacaoPorTipo'
import { GraficoRentabilidade } from './GraficoRentabilidade'
import { GraficoValores } from './GraficoValores'

function LegendaSetores({ itens }) {
  return (
    <ul className="mt-4 space-y-2">
      {itens.map((item) => (
        <li key={item.setor} className="rounded-lg border border-slate-800 bg-slate-900/40 px-3 py-2">
          <div className="flex items-center justify-between gap-2 text-sm">
            <div className="flex items-center gap-2 text-slate-200">
              <span className="h-2.5 w-2.5 rounded-full" style={{ backgroundColor: item.cor }} />
              <span className="font-medium">{item.setor}</span>
            </div>
            <span className="text-xs font-medium text-slate-300">{item.percentual}%</span>
          </div>
          {item.ativos?.length > 0 && (
            <div className="mt-1.5 flex flex-wrap gap-1.5 pl-4.5">
              {item.ativos.map((ativo) => (
                <span
                  key={ativo.ticker}
                  className="rounded-md bg-slate-800/80 px-1.5 py-0.5 text-[11px] font-medium text-slate-300"
                >
                  {ativo.ticker}
                </span>
              ))}
            </div>
          )}
        </li>
      ))}
    </ul>
  )
}

export function CarteiraGraficos({ dadosGraficos }) {
  if (!dadosGraficos.temDados) {
    return <EmptyState>Adicione ativos na aba Carteira para visualizar os gráficos.</EmptyState>
  }

  return (
    <section className="grid gap-4 lg:grid-cols-2">
      <Panel titulo="Alocação por ativo">
        <div className="p-4">
          <GraficoAlocacao dados={dadosGraficos.alocacao} />
          <ul className="mt-4 flex flex-wrap justify-center gap-x-4 gap-y-2">
            {dadosGraficos.alocacao.map((item) => (
              <li key={item.ticker} className="flex items-center gap-2 text-xs text-slate-300">
                <span className="h-2.5 w-2.5 rounded-full" style={{ backgroundColor: item.cor }} />
                {item.ticker} ({item.percentual}%)
              </li>
            ))}
          </ul>
        </div>
      </Panel>

      <Panel titulo="Alocação por tipo de ativo">
        <div className="p-4">
          <GraficoAlocacaoPorTipo dados={dadosGraficos.alocacaoPorTipo} />
          <ul className="mt-4 flex flex-wrap justify-center gap-x-4 gap-y-2">
            {dadosGraficos.alocacaoPorTipo.map((item) => (
              <li key={item.tipo} className="flex items-center gap-2 text-xs text-slate-300">
                <span className="h-2.5 w-2.5 rounded-full" style={{ backgroundColor: item.cor }} />
                {item.rotulo} ({item.percentual}%)
              </li>
            ))}
          </ul>
        </div>
      </Panel>

      {dadosGraficos.alocacaoPorSetorAcoes.length > 0 && (
        <Panel titulo="Setores - Ações">
          <div className="p-4">
            <GraficoAlocacaoPorSetor dados={dadosGraficos.alocacaoPorSetorAcoes} />
            <LegendaSetores itens={dadosGraficos.alocacaoPorSetorAcoes} />
          </div>
        </Panel>
      )}

      {dadosGraficos.alocacaoPorSetorFiis.length > 0 && (
        <Panel titulo="Setores - FIIs">
          <div className="p-4">
            <GraficoAlocacaoPorSetor dados={dadosGraficos.alocacaoPorSetorFiis} />
            <LegendaSetores itens={dadosGraficos.alocacaoPorSetorFiis} />
          </div>
        </Panel>
      )}

      <Panel titulo="Investido vs valor atual">
        <div className="p-4 pb-2">
          <GraficoValores dados={dadosGraficos.comparativo} />
          <div className="mt-2 flex justify-center gap-6 text-xs text-slate-400">
            <span className="flex items-center gap-2">
              <span className="h-2.5 w-2.5 rounded-sm bg-slate-500" />
              Investido
            </span>
            <span className="flex items-center gap-2">
              <span className="h-2.5 w-2.5 rounded-sm bg-violet-500" />
              Atual
            </span>
          </div>
        </div>
      </Panel>

      <Panel titulo="Rentabilidade por ativo (%)" className="lg:col-span-2">
        <div className="p-4">
          <GraficoRentabilidade dados={dadosGraficos.rentabilidade} />
        </div>
      </Panel>
    </section>
  )
}
