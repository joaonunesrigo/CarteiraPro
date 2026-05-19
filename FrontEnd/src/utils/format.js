export const formatarMoeda = (valor) =>
  new Intl.NumberFormat("pt-BR", {
    style: "currency",
    currency: "BRL",
  }).format(valor);

export const formatarPercentual = (valor) =>
  `${valor >= 0 ? "+" : ""}${Number(valor).toFixed(2)}%`;
