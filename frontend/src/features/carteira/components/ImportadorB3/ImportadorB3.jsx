import { Button } from '../../../../components/Button'
import { Card } from '../../../../components/Card'
import { Select } from '../../../../components/Select'
import { TIPOS_ATIVO } from '../../constants/tiposAtivo'

export function ImportadorB3({
  linhas,
  processandoArquivo,
  importando,
  erroArquivo,
  nomeArquivo,
  totalSelecionadas,
  aoSelecionarArquivo,
  atualizarLinha,
  alternarSelecionado,
  aplicarPrecoFechamentoComoPm,
  limparPreview,
  confirmarImportacao,
}) {
  return (
    <Card titulo="Importar da B3" className="mb-8">
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
        , acesse{' '}
        <strong className="text-slate-300">
          Minhas carteiras → Investimentos → Posição
        </strong>
        , filtre e baixe em{' '}
        <strong className="text-slate-300">Excel (.xlsx)</strong>. O arquivo não
        traz preço médio — preencha na tabela abaixo.
      </p>

      <div className="flex flex-wrap items-center gap-3">
        <label className="cursor-pointer">
          <span className="inline-block rounded-lg border border-slate-600 bg-slate-800 px-4 py-2 text-sm font-medium text-slate-200 transition hover:bg-slate-700">
            {processandoArquivo ? 'Lendo arquivo…' : 'Escolher arquivo .xlsx'}
          </span>
          <input
            type="file"
            accept=".xlsx,application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            className="sr-only"
            disabled={processandoArquivo || importando}
            onChange={aoSelecionarArquivo}
          />
        </label>

        {nomeArquivo && (
          <span className="text-sm text-slate-500">{nomeArquivo}</span>
        )}

        {linhas.length > 0 && (
          <Button variante="secondary" type="button" onClick={limparPreview}>
            Limpar
          </Button>
        )}
      </div>

      {erroArquivo && (
        <p className="mt-3 text-sm text-red-400" role="alert">
          {erroArquivo}
        </p>
      )}

      {linhas.length > 0 && (
        <div className="mt-6">
          <div className="mb-4 flex flex-wrap items-center gap-3">
            <Button
              variante="secondary"
              type="button"
              onClick={aplicarPrecoFechamentoComoPm}
            >
              Usar preço de fechamento como PM
            </Button>
            <p className="text-xs text-amber-400/90">
              O preço de fechamento é cotação de mercado, não o preço médio real
              de compra.
            </p>
          </div>

          <div className="overflow-x-auto rounded-lg border border-slate-700">
            <table className="w-full min-w-[640px] text-left text-sm">
              <thead className="border-b border-slate-700 bg-slate-800/80 text-slate-400">
                <tr>
                  <th className="w-10 px-3 py-2" />
                  <th className="px-3 py-2">Ticker</th>
                  <th className="px-3 py-2">Qtd</th>
                  <th className="px-3 py-2">PM (R$)</th>
                  <th className="px-3 py-2">Fech.</th>
                  <th className="px-3 py-2">Tipo</th>
                  <th className="px-3 py-2">Aba</th>
                </tr>
              </thead>
              <tbody>
                {linhas.map((linha) => (
                  <tr
                    key={linha.id}
                    className={`border-b border-slate-800 ${
                      linha.jaCadastrado ? 'opacity-50' : ''
                    }`}
                  >
                    <td className="px-3 py-2">
                      <input
                        type="checkbox"
                        checked={linha.selecionado}
                        disabled={linha.jaCadastrado}
                        onChange={() => alternarSelecionado(linha.id)}
                        className="rounded border-slate-600"
                      />
                    </td>
                    <td className="px-3 py-2 font-medium text-white">
                      {linha.ticker}
                      {linha.jaCadastrado && (
                        <span className="ml-2 text-xs text-slate-500">
                          já na carteira
                        </span>
                      )}
                    </td>
                    <td className="px-3 py-2 text-slate-300">
                      {linha.quantidade}
                    </td>
                    <td className="px-3 py-2">
                      <input
                        type="number"
                        step="0.01"
                        min="0"
                        disabled={linha.jaCadastrado}
                        value={linha.precoMedio}
                        onChange={(e) =>
                          atualizarLinha(linha.id, 'precoMedio', e.target.value)
                        }
                        placeholder="0,00"
                        className="w-24 rounded border border-slate-600 bg-slate-900 px-2 py-1 text-white disabled:opacity-50"
                      />
                    </td>
                    <td className="px-3 py-2 text-slate-500">
                      {linha.precoFechamento != null
                        ? linha.precoFechamento.toLocaleString('pt-BR', {
                            minimumFractionDigits: 2,
                          })
                        : '—'}
                    </td>
                    <td className="px-3 py-2">
                      <Select
                        disabled={linha.jaCadastrado}
                        value={linha.tipo}
                        onChange={(e) =>
                          atualizarLinha(
                            linha.id,
                            'tipo',
                            Number(e.target.value),
                          )
                        }
                        className="min-w-[7rem]"
                      >
                        {TIPOS_ATIVO.map((t) => (
                          <option key={t.valor} value={t.valor}>
                            {t.rotulo}
                          </option>
                        ))}
                      </Select>
                    </td>
                    <td className="px-3 py-2 text-xs text-slate-500">
                      {linha.origemAba}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          <div className="mt-4 flex justify-end">
            <Button
              type="button"
              disabled={importando || totalSelecionadas === 0}
              onClick={confirmarImportacao}
            >
              {importando
                ? 'Importando…'
                : `Importar ${totalSelecionadas} ativo(s)`}
            </Button>
          </div>
        </div>
      )}
    </Card>
  )
}
