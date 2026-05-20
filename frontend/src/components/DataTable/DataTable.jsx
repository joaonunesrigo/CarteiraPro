import { Paginacao, usePaginacao } from '../Paginacao'
import { Panel } from '../Panel'

export function DataTable({
  titulo,
  acao,
  colunas,
  itens,
  obterChaveLinha,
  estadoVazio = 'Nenhum item encontrado.',
  estadoCarregando = 'Carregando...',
  carregando = false,
  paginavel = true,
  tamanhoPagina = 10,
  minWidthClasse = 'min-w-[640px]',
  className,
}) {
  const paginacao = usePaginacao(itens, tamanhoPagina)
  const itensExibidos = paginavel ? paginacao.itensPagina : itens

  return (
    <Panel titulo={titulo} acao={acao} className={className}>
      {carregando ? (
        <p className="px-4 py-6 text-sm text-slate-400">{estadoCarregando}</p>
      ) : itens.length === 0 ? (
        <p className="px-4 py-6 text-center text-sm text-slate-400">
          {estadoVazio}
        </p>
      ) : (
        <>
          <div className="overflow-x-auto">
            <table className={`w-full ${minWidthClasse} text-left text-sm`}>
              <thead className="bg-slate-900 text-slate-400">
                <tr>
                  {colunas.map((coluna) => (
                    <th
                      key={coluna.chave}
                      className={`px-4 py-3 font-medium ${coluna.classeCabecalho ?? ''}`}
                    >
                      {coluna.titulo}
                    </th>
                  ))}
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-800 bg-slate-900/30">
                {itensExibidos.map((item) => (
                  <tr key={obterChaveLinha(item)} className="text-slate-200">
                    {colunas.map((coluna) => {
                      const classeCelula =
                        typeof coluna.classeCelula === 'function'
                          ? coluna.classeCelula(item)
                          : (coluna.classeCelula ?? '')

                      return (
                        <td
                          key={coluna.chave}
                          className={`px-4 py-3 ${classeCelula}`}
                        >
                          {coluna.render(item)}
                        </td>
                      )
                    })}
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          {paginavel && (
            <div className="border-t border-slate-800 px-4 py-3">
              <Paginacao {...paginacao} className="mt-0" />
            </div>
          )}
        </>
      )}
    </Panel>
  )
}
