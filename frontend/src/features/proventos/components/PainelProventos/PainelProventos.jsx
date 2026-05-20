import { Alert } from '../../../../components/Alert'
import { Button } from '../../../../components/Button'
import { Card } from '../../../../components/Card'
import { TIPOS_PROVENTO } from '../../constants/tiposProvento'

const moeda = new Intl.NumberFormat('pt-BR', {
  style: 'currency',
  currency: 'BRL',
})

const numero = new Intl.NumberFormat('pt-BR', {
  minimumFractionDigits: 2,
  maximumFractionDigits: 8,
})

function formatarData(data) {
  return new Date(data).toLocaleDateString('pt-BR', { timeZone: 'UTC' })
}

function obterDataUtc(data) {
  const valor = new Date(data)
  return new Date(
    Date.UTC(valor.getUTCFullYear(), valor.getUTCMonth(), valor.getUTCDate()),
  )
}

function formatarMesAno(data) {
  return data.toLocaleDateString('pt-BR', {
    month: 'short',
    year: 'numeric',
    timeZone: 'UTC',
  })
}

function somarProventos(proventos, filtro) {
  return proventos
    .filter(filtro)
    .reduce((total, provento) => total + Number(provento.valorTotal ?? 0), 0)
}

function calcularIndicadores(proventos) {
  if (proventos.length === 0) {
    return {
      periodo: 'Sem proventos importados',
      totalMesAtual: 0,
      totalUltimos12Meses: 0,
    }
  }

  const datas = proventos.map((provento) => obterDataUtc(provento.dataPagamento))
  const primeiraData = new Date(Math.min(...datas))
  const ultimaData = new Date(Math.max(...datas))
  const hoje = new Date()
  const inicioMesAtual = new Date(
    Date.UTC(hoje.getUTCFullYear(), hoje.getUTCMonth(), 1),
  )
  const inicioUltimos12Meses = new Date(
    Date.UTC(
      hoje.getUTCFullYear() - 1,
      hoje.getUTCMonth(),
      hoje.getUTCDate(),
    ),
  )

  return {
    periodo: `${formatarMesAno(primeiraData)} - ${formatarMesAno(ultimaData)}`,
    totalMesAtual: somarProventos(proventos, (provento) => {
      const data = obterDataUtc(provento.dataPagamento)
      return data >= inicioMesAtual
    }),
    totalUltimos12Meses: somarProventos(proventos, (provento) => {
      const data = obterDataUtc(provento.dataPagamento)
      return data >= inicioUltimos12Meses
    }),
  }
}

export function PainelProventos({
  proventos,
  resumo,
  carregando,
  erro,
  importador,
}) {
  const indicadores = calcularIndicadores(proventos)

  return (
    <div className="space-y-8">
      {erro && <Alert>{erro}</Alert>}

      <Card titulo="Importar proventos da B3">
        <p className="mb-4 text-sm leading-relaxed text-slate-400">
          No portal{' '}
          <a
            href="https://www.investidor.b3.com.br/"
            target="_blank"
            rel="noreferrer"
            className="text-violet-400 underline hover:text-violet-300"
          >
            investidor.b3.com.br
          </a>
          , baixe o relatório{' '}
          <strong className="text-slate-300">Proventos Recebidos</strong> ou{' '}
          <strong className="text-slate-300">Movimentação</strong> em Excel
          (.xlsx). São importados apenas Dividendos, JCP e Rendimentos dos
          tickers que já existem na sua carteira.
        </p>

        <div className="flex flex-wrap items-center gap-3">
          <label className="cursor-pointer">
            <span className="inline-block rounded-lg border border-slate-600 bg-slate-800 px-4 py-2 text-sm font-medium text-slate-200 transition hover:bg-slate-700">
              {importador.processandoArquivo
                ? 'Lendo arquivo...'
                : 'Escolher .xlsx da B3'}
            </span>
            <input
              type="file"
              accept=".xlsx,application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
              className="sr-only"
              disabled={importador.processandoArquivo || importador.importando}
              onChange={importador.aoSelecionarArquivo}
            />
          </label>

          {importador.nomeArquivo && (
            <span className="text-sm text-slate-500">
              {importador.nomeArquivo}
            </span>
          )}

          {importador.linhas.length > 0 && (
            <Button
              variante="secondary"
              type="button"
              onClick={importador.limparPreview}
            >
              Limpar
            </Button>
          )}
        </div>

        {importador.erroArquivo && (
          <p className="mt-3 text-sm text-red-400" role="alert">
            {importador.erroArquivo}
          </p>
        )}

        {importador.linhas.length > 0 && (
          <div className="mt-6">
            {(() => {
              const foraDaCarteira = importador.linhas.filter(
                (linha) => !linha.ativoCadastrado,
              )
              if (foraDaCarteira.length === 0) return null
              const tickers = [
                ...new Set(foraDaCarteira.map((linha) => linha.ticker)),
              ]
              const somaFora = foraDaCarteira.reduce(
                (total, linha) => total + Number(linha.valorTotal ?? 0),
                0,
              )
              return (
                <p className="mb-4 rounded-md border border-violet-500/30 bg-violet-500/10 px-3 py-2 text-xs text-violet-200">
                  {foraDaCarteira.length} lançamento(s) totalizando{' '}
                  {moeda.format(somaFora)} pertencem a ativos que não estão na
                  carteira atual ({tickers.join(', ')}). Eles serão importados
                  como histórico, sem voltar para a carteira.
                </p>
              )
            })()}

            <div className="overflow-x-auto rounded-lg border border-slate-700">
              <table className="w-full min-w-[760px] text-left text-sm">
                <thead className="border-b border-slate-700 bg-slate-800/80 text-slate-400">
                  <tr>
                    <th className="w-10 px-3 py-2" />
                    <th className="px-3 py-2">Data</th>
                    <th className="px-3 py-2">Ticker</th>
                    <th className="px-3 py-2">Tipo</th>
                    <th className="px-3 py-2">Qtd</th>
                    <th className="px-3 py-2">Valor/cota</th>
                    <th className="px-3 py-2">Total</th>
                    <th className="px-3 py-2">Status</th>
                  </tr>
                </thead>
                <tbody>
                  {importador.linhas.map((linha) => (
                    <tr key={linha.id} className="border-b border-slate-800">
                      <td className="px-3 py-2">
                        <input
                          type="checkbox"
                          checked={linha.selecionado}
                          disabled={linha.jaImportado}
                          onChange={() => importador.alternarSelecionado(linha.id)}
                          className="rounded border-slate-600"
                        />
                      </td>
                      <td className="px-3 py-2 text-slate-300">
                        {formatarData(linha.dataPagamento)}
                      </td>
                      <td className="px-3 py-2 font-medium text-white">
                        {linha.ticker}
                      </td>
                      <td className="px-3 py-2 text-slate-300">
                        {TIPOS_PROVENTO[linha.tipo] ?? 'Provento'}
                      </td>
                      <td className="px-3 py-2 text-slate-300">
                        {numero.format(linha.quantidade)}
                      </td>
                      <td className="px-3 py-2 text-slate-300">
                        {moeda.format(linha.valorPorCota)}
                      </td>
                      <td className="px-3 py-2 font-medium text-emerald-300">
                        {moeda.format(linha.valorTotal)}
                      </td>
                      <td className="px-3 py-2 text-xs text-slate-500">
                        {linha.jaImportado
                            ? 'já importado'
                            : linha.ativoCadastrado
                              ? 'pronto'
                              : 'histórico'}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>

            <div className="mt-4 flex justify-end">
              <Button
                type="button"
                disabled={
                  importador.importando || importador.totalSelecionadas === 0
                }
                onClick={importador.confirmarImportacao}
              >
                {importador.importando
                  ? 'Importando...'
                  : `Importar ${importador.totalSelecionadas} provento(s)`}
              </Button>
            </div>
          </div>
        )}
      </Card>

      <div className="grid gap-4 md:grid-cols-3">
        <Card titulo="Total recebido (todo o período)">
          <p className="text-2xl font-semibold text-emerald-300">
            {moeda.format(resumo?.totalRecebido ?? 0)}
          </p>
          <p className="mt-1 text-xs text-slate-500">
            Período: {indicadores.periodo}
          </p>
        </Card>
        <Card titulo="Recebido no mês atual">
          <p className="text-2xl font-semibold text-emerald-300">
            {moeda.format(indicadores.totalMesAtual)}
          </p>
          <p className="mt-1 text-xs text-slate-500">
            Proventos pagos desde o dia 1
          </p>
        </Card>
        <Card titulo="Recebido nos últimos 12 meses">
          <p className="text-2xl font-semibold text-emerald-300">
            {moeda.format(indicadores.totalUltimos12Meses)}
          </p>
          <p className="mt-1 text-xs text-slate-500">
            Janela mensal móvel
          </p>
        </Card>
      </div>

      <Card titulo="Proventos importados">
        {carregando ? (
          <p className="text-slate-400">Carregando proventos...</p>
        ) : proventos.length === 0 ? (
          <p className="text-sm text-slate-400">
            Nenhum provento importado ainda.
          </p>
        ) : (
          <div className="overflow-x-auto rounded-lg border border-slate-700">
            <table className="w-full min-w-[640px] text-left text-sm">
              <thead className="border-b border-slate-700 bg-slate-800/80 text-slate-400">
                <tr>
                  <th className="px-3 py-2">Data</th>
                  <th className="px-3 py-2">Ticker</th>
                  <th className="px-3 py-2">Tipo</th>
                  <th className="px-3 py-2">Qtd</th>
                  <th className="px-3 py-2">Valor/cota</th>
                  <th className="px-3 py-2">Total</th>
                </tr>
              </thead>
              <tbody>
                {proventos.map((provento) => (
                  <tr key={provento.id} className="border-b border-slate-800">
                    <td className="px-3 py-2 text-slate-300">
                      {formatarData(provento.dataPagamento)}
                    </td>
                    <td className="px-3 py-2 font-medium text-white">
                      {provento.ticker}
                      {!provento.ativoId && (
                        <span className="ml-2 text-xs font-normal text-slate-500">
                          histórico
                        </span>
                      )}
                    </td>
                    <td className="px-3 py-2 text-slate-300">
                      {TIPOS_PROVENTO[provento.tipo] ?? 'Provento'}
                    </td>
                    <td className="px-3 py-2 text-slate-300">
                      {numero.format(provento.quantidade)}
                    </td>
                    <td className="px-3 py-2 text-slate-300">
                      {moeda.format(provento.valorPorCota)}
                    </td>
                    <td className="px-3 py-2 font-medium text-emerald-300">
                      {moeda.format(provento.valorTotal)}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </Card>
    </div>
  )
}
