import { useEffect, useMemo, useState } from 'react'

export const OPCOES_TAMANHO_PAGINA = [10, 25, 50, 100]

export function usePaginacao(itens, tamanhoInicial = 10) {
  const [tamanhoPagina, setTamanhoPagina] = useState(tamanhoInicial)
  const [paginaAtual, setPaginaAtual] = useState(1)

  const totalItens = itens.length
  const totalPaginas = Math.max(1, Math.ceil(totalItens / tamanhoPagina))

  useEffect(() => {
    if (paginaAtual > totalPaginas) {
      setPaginaAtual(totalPaginas)
    }
  }, [paginaAtual, totalPaginas])

  const itensPagina = useMemo(() => {
    const inicio = (paginaAtual - 1) * tamanhoPagina
    return itens.slice(inicio, inicio + tamanhoPagina)
  }, [itens, paginaAtual, tamanhoPagina])

  function irParaPagina(pagina) {
    const seguro = Math.min(Math.max(1, pagina), totalPaginas)
    setPaginaAtual(seguro)
  }

  function mudarTamanhoPagina(novo) {
    setTamanhoPagina(novo)
    setPaginaAtual(1)
  }

  return {
    itensPagina,
    paginaAtual,
    totalPaginas,
    totalItens,
    tamanhoPagina,
    irParaPagina,
    mudarTamanhoPagina,
  }
}
