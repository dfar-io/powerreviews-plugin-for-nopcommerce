# Update this to support all versions of NOP the plugin supports

rm -rf 4.40/nopSource
wget https://github.com/nopSolutions/nopCommerce/releases/download/release-4.40.4/nopCommerce_4.40.4_Source.zip
unzip nopCommerce_4.40.4_Source.zip -d 4.40/nopSource
rm nopCommerce_4.40.4_Source.zip